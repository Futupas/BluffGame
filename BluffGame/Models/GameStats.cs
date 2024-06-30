namespace BluffGame.Models;

public class GameStats
{
    public IReadOnlyList<(string username, double rate)> GuessRates { get; init; }
    public IReadOnlyList<(string username, double rate)> LieRates { get; init; }
    public IReadOnlyList<(string who, string toWhom, double rate)> GuessCouplesRates { get; init; }
    public IReadOnlyList<(string who, string toWhom, double rate)> LieCouplesRates { get; init; }
}