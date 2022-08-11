namespace Gravy.ConsoleString.Tags;

public class ResetAllInStrictModeException : TagsCompilerException
{
    public ResetAllInStrictModeException(int tokenLine, int tokenColumn) : base(tokenLine, tokenColumn)
    {
    }
    
    public override string Message => $"ResetAll is not allowed in strict mode at {Line}:{Column}.";
}