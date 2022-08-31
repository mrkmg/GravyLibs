namespace Gravy.ConsoleString.Tags;

internal class UnmatchedStopTokenException : TagsCompilerException
{
    private readonly string _token;

    public UnmatchedStopTokenException(string token, int tokenLine, int tokenColumn) : base(tokenLine, tokenColumn)
    {
        _token = token;
    }

    public override string Message => $"Unmatched stop token '{_token}' at {Line}:{Column}";
}