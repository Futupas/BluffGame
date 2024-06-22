namespace BluffGame.Models;

public record Round
{
    public List<Couple> Couples { get; init; } = new();
    public bool IsFinished { get; set; }
}
