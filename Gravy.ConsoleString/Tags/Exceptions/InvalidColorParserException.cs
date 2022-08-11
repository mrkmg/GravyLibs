namespace Gravy.ConsoleString.Tags;

public class InvalidColorParserException : TagsCompilerException
{
    public char Parser { get; }

    public InvalidColorParserException(char parser, int line, int column) : base(line, column)
    {
        Parser = parser;
    }

    public override string Message => $"Invalid Color Parser '{Parser}' at {Line}:{Column}";
}