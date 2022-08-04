namespace Gravy.ConsoleString.Parser;

public class MissingCloseBracketException : ParserException
{
    public MissingCloseBracketException(int index)
    {
        Index = index;
    }

    public override string Message => $"Expected ']' at {Index}";
}