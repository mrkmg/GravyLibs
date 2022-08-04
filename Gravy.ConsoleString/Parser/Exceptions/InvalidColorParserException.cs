namespace Gravy.ConsoleString.Parser;

public class InvalidColorParserException : ParserException
{
    public char Parser { get; }

    public InvalidColorParserException(char parser, int index)
    {
        Parser = parser;
        Index = index;
    }

    public override string Message => $"Invalid Color Parser '{Parser}' at {Index}";
}