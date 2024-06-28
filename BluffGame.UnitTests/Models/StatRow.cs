namespace BluffGame.UnitTests.Models;

public class StatsRow
{
    public int All { get; set; } = 0;
    
    /// <summary> Can be 'Lied' or 'Guessed'</summary>
    public int True { get; set; } = 0;

    public double GetRate() => (double)True / All;
}

