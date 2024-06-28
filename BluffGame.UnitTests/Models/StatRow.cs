namespace BluffGame.UnitTests.Models;

public class StatisticsRow
{
    public int All { get; set; } = 0;
    public int Guessed { get; set; } = 0;
    // todo Lie

    public double GetGuessRate() => (double)Guessed / All;
}

// public abstract record StatRow
// {
//     public string Username { get; set; }
//     public int RoundId { get; set; }
//     public bool Value { get; set; }
// }
//
// public record StatRowWish : StatRow
// {
//     public bool? Hint { get; set; }
//     public bool Lie => Hint is not null && Hint.Value != Value;
// }
//
// public record StatRowAnswer : StatRow
// {
//     
// }
