namespace Gravy.ConsoleString.Parser;

public class UnknownTagException : ParserException
{
    public char Tag { get; }

    public UnknownTagException(char tag, int index)
    {
        Tag = tag;
        Index = index;
    }

    public override string Message => $"Unknown Tag [{Tag}] at {Index}";
}