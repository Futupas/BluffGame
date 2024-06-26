using BluffGame.Pages;

namespace BluffGame.Models;

//todo remove games after something

public class Game
{
    public static Dictionary<string, Game> Games { get; } = new();


    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid CreatorGuid { get; private init; } = Guid.NewGuid();

    public GameStatus Status { get; set; } = GameStatus.WaitingForCreator;

    public Dictionary<string, GamePage> Users { get; } = new();

    public List<Round> Rounds { get; } = new();
    
    
    private IEnumerable<Couple> GetArchiveCouples()
    {
        foreach (var round in Rounds)
        {
            if (!round.IsFinished) continue;
            foreach (var couple in round.Couples)
            {
                if (!couple.IsFinished) continue; // It's impossible, but still
                yield return couple;
            }
        }
    }

    public double GetUserGuessRate(string user)
    {
        var total = GetArchiveCouples().Where(x => x.UserAnswers == user);
        var guessed = total.Where(x => x.Guessed);

        var totalCount = total.Count();
        if (totalCount == 0) return 0;

        return (double)guessed.Count() / totalCount;
    }
    public double GetUserLieRate(string user)
    {
        var total = GetArchiveCouples().Where(x => x.UserAsks == user && x.Lied is not null);
        var lied = total.Where(x => x.Lied == true);

        var totalCount = total.Count();
        if (totalCount == 0) return 0;
        
        return (double)lied.Count() / totalCount;
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