namespace BluffGame.Exceptions;

public class UnreachableException: Exception
{
    public UnreachableException() { }

    public UnreachableException(string message): base(message) { }
}
