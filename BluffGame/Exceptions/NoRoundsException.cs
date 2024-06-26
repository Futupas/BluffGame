namespace BluffGame.Exceptions;

public class NoRoundsException: UnreachableException
{
    public NoRoundsException(): base("No rounds in game") { }
}
