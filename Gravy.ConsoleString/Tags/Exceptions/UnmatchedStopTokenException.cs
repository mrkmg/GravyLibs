namespace Gravy.ConsoleString.Tags;

internal class UnmatchedStopTokenException : TagsCompilerException
{
    public UnmatchedStopTokenException(int tokenLine, int tokenColumn) : base(tokenLine, tokenColumn)
    {
    }

    public override string Message => $"Unmatched stop token at {Line}:{Column}";
}