namespace BluffGame.Models;

public record Round
{
    public List<Couple> Couples { get; init; } = new();
    
    public bool IsArchived => AllWished && AllAnswered;

    public bool AllWished => Couples.All(x => x.Wished is not null);
    public bool AllAnswered => Couples.All(x => x.Answered is not null);
}
