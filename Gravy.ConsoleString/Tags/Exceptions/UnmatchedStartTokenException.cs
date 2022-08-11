namespace Gravy.ConsoleString.Tags;

public class UnmatchedStartTokenException : TagsCompilerException
{
    public UnmatchedStartTokenException(int tokenLine, int tokenColumn) : base(tokenLine, tokenColumn)
    {
    }
    
    public override string Message => $"Unmatched start token at line {Line}:{Column}";
}