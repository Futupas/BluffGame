using System.Text.Json.Serialization;

namespace BluffGame.Models;

public class QuestionModel
{
    public string Question { get; init; }
    
    public string Option1 { get; init; }
    
    public string Option2 { get; init; }
}
