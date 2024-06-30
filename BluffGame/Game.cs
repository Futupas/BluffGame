using BluffGame.Exceptions;
using BluffGame.Models;
using BluffGame.Pages;

namespace BluffGame;

//todo remove games after something

// You ALWAYS
// * WISH for value in LAST round
// * Answer for a value in Round[^2] (2ND LAST)

public class Game
{
    private readonly IConfiguration _config;
    public static Dictionary<string, Game> Games { get; } = new();


    public string Id { get; private init; }
    public Guid CreatorGuid { get; private init; } = Guid.NewGuid();

    public GameStatus Status { get; set; } = GameStatus.WaitingForCreator;

    public Dictionary<string, GamePage> Users { get; } = new();

    public List<Round> Rounds { get; } = new();

    public Game(IConfiguration config)
    {
        _config = config;
        Id = Helpers.GenerateShortId();
        while(Games.ContainsKey(Id)) Id = Helpers.GenerateShortId();
        Games[Id] = this;
    }

    public double GetUserGuessRate(string user)
    {
        var total = Rounds
            .SelectMany(x => x.Couples)
            .Where(x => x.UserAnswers == user && x.Wished is not null && x.Answered is not null);
        var guessed = total.Where(x => x.Guessed);

        var totalCount = total.Count();
        if (totalCount == 0) return 0;

        return (double)guessed.Count() / totalCount;
    }
    public double GetUserLieRate(string user)
    {
        var total = Rounds
            .SelectMany(x => x.Couples)
            .Where(x => x.UserAsks == user && x.Wished is not null);
        var lied = total.Where(x => x.Lied == true);

        var totalCount = total.Count();
        if (totalCount == 0) return 0;
        
        return (double)lied.Count() / totalCount;
    }

    public GameStats? GetStatistics()
    {
        var guessRates = new List<(string username, double rate)>();
        var lieRates = new List<(string username, double rate)>();
        var guessCouplesRates = new List<(string who, string toWhom, double rate)>();
        var lieCouplesRates = new List<(string who, string toWhom, double rate)>();

        if (Rounds.Count < 3) return null;
        
        foreach (var username in Users.Keys)
        {
            guessRates.Add((username, GetUserGuessRate(username)));
            lieRates.Add((username, GetUserLieRate(username)));
        }

        var lieCouples = Rounds.SelectMany(x => x.Couples).Where(x => x.Wished is not null);
        var guessCouples = Rounds.SelectMany(x => x.Couples).Where(x => x.Answered is not null);
        
        foreach (var who in Users.Keys)
        {
            foreach (var toWhom in Users.Keys)
            {
                if (who == toWhom) continue;

                // Lie
                {
                    var all = lieCouples.Where(x => x.UserAsks == who && x.UserAnswers == toWhom);
                    var lied = all.Where(x => x.Lied == true);
                    
                    lieCouplesRates.Add((who, toWhom, (double)all.Count() / lied.Count()));
                }
                
                //Guess
                {
                    var all = guessCouples.Where(x => x.UserAnswers == who && x.UserAsks == toWhom);
                    var guessed = all.Where(x => x.Guessed);
                    
                    guessCouplesRates.Add((who, toWhom, (double)all.Count() / guessed.Count()));
                }

            }
        }

        return new GameStats()
        {
            GuessRates = guessRates.OrderByDescending(x => x.rate).ToList(),
            LieRates = lieRates.OrderByDescending(x => x.rate).ToList(),
            GuessCouplesRates = guessCouplesRates.OrderByDescending(x => x.rate).ToList(),
            LieCouplesRates = lieCouplesRates.OrderByDescending(x => x.rate).ToList(),
        };
    }

    public bool HaveIWished(string user)
    {
        if (!Rounds.Any()) throw new NoRoundsException();
        
        return Rounds
            .Last()
            .Couples
            .Exists(x => x.UserAsks == user && x.Wished is not null);

    }
    public bool HaveIAnswered(string user)
    {
        if (Rounds.Count < 2) return true;
        
        return Rounds[^2]
            .Couples
            .Exists(x => x.UserAnswers == user && x.Answered is not null);

    }

    public bool TryFindDidIWinInPreviousRound(string user, out bool win)
    {
        var result = Rounds
            .LastOrDefault(x => x.Couples.Exists(y => y.UserAnswers == user && y.Answered is not null))?
            .Couples
            .First(x => x.UserAnswers == user)
            .Guessed;

        win = result ?? false;
        return result is not null;
    }

    public Round NewRound()
    {
        var users = Users.Keys.ToList();
        Helpers.ShuffleList(users);
        var couples = new List<Couple>(users.Count);
        for (int i = 0; i < users.Count; i++)
        {
            couples.Add(new()
            {
                UserAsks = users[i],
                UserAnswers = users[(i + 1) % users.Count],
            });
        }

        // var questions = _config.GetSection("questions").Get<List<QuestionModel>>();
        // var questionIndex = Random.Shared.Next(questions.Count);
        // var question = questions[questionIndex];

        var question = Helpers.RandomChoose(_config.GetSection("onlyQuestions").Get<IReadOnlyList<string>>());
        var options = Helpers.RandomChoose2(_config.GetSection("onlyAnswers").Get<IReadOnlyList<string>>());

        var round = new Round() { Couples = couples, Question = new()
        {
            Question = question,
            Option1 = options.option1,
            Option2 = options.option2,
        } };
        Rounds.Add(round);

        return round;
    }

    public string? WhosValueAmIGuessing(string username)
    {
        // todo also return some data
        if (Rounds.Count < 2) return null;
        return Rounds[^2].Couples.First(x => x.UserAnswers == username).UserAsks;
    }
    public bool TryGetMyHint(string username, out bool hint)
    {
        if (Rounds.Count < 2)
        {
            hint = false;
            return false;
        }

        var realHint = Rounds[^2].Couples.First(x => x.UserAnswers == username).Hint;

        if (realHint is null)
        {
            hint = false;
            return false;
        }

        hint = realHint.Value;
        return true;

    }
    public string WhoAmIGuessingFor(string username)
    {
        if (!Rounds.Any()) throw new NoRoundsException();

        return Rounds.Last().Couples.First(x => x.UserAsks == username).UserAnswers;
    }

    public void WishValue(string username, bool value, bool? hint)
    {
        if (!Rounds.Any()) throw new NoRoundsException();
        var couple = Rounds.Last().Couples.First(x => x.UserAsks == username);
        couple.Wished = value;
        couple.Hint = hint;
    }
    public void AnswerValue(string username, bool value)
    {
        if (Rounds.Count < 2) throw new NoRoundsException();
        Rounds[^2].Couples.First(x => x.UserAnswers == username).Answered = value;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not Game that) return false;
        return this.Id == that.Id;
    }

    public override string ToString()
    {
        return Id.ToString();
    }
}