namespace BluffGame.Models;

//todo mb forbid changes after finished

public class Couple
{
    public string UserAsks { get; init; }
    public string UserAnswers { get; init; }
    
    /// <summary> Raw value </summary>
    public bool? Wished { get; set; } = null;
    /// <summary> Raw value </summary>
    public bool? Answered { get; set; } = null;
    /// <summary> Raw value </summary>
    public bool? Hint { get; set; }

    /// <summary> Calculated value </summary>
    public bool? Lied => Hint is null ? null : (Hint != Wished);

    /// <summary> Calculated value </summary>
    public bool Guessed => Wished is not null && Answered is not null && Wished == Answered;
    
}
