namespace Gravy.ConsoleString.Tags;

public class MissingCloseBracketException : TagsCompilerException
{
    public MissingCloseBracketException(int line, int column) : base(line, column) { }
    
    public override string Message => $"Expected ']' at {Line}:{Column}";
}