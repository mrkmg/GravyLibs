namespace Gravy.ConsoleString.Tags;

public class UnknownTagException : TagsCompilerException
{
    public char Tag { get; }

    public UnknownTagException(char tag, int line, int column) : base(line, column)
    {
        Tag = tag;
    }

    public override string Message => $"Unknown Tag [{Tag}] at {Line}:{Column}";
}