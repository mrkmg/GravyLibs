namespace Gravy.ConsoleString.Tags;

public class UnexpectedEndOfStringException : TagsCompilerException
{
    public UnexpectedEndOfStringException(int line, int column) : base(line, column) { }
    public override string Message => $"Unexpected end of string at when parsing token at {Line}:{Column}";
}