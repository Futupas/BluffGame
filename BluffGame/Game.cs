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


    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid CreatorGuid { get; private init; } = Guid.NewGuid();

    public GameStatus Status { get; set; } = GameStatus.WaitingForCreator;

    public Dictionary<string, GamePage> Users { get; } = new();

    public List<Round> Rounds { get; } = new();

    public Game(IConfiguration config)
    {
        _config = config;
        Games[Id.ToString()] = this;
    }
    
    
    private IEnumerable<Couple> GetArchiveCouples()
    {
        return Rounds.Where(round => round.IsArchived).SelectMany(round => round.Couples);
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