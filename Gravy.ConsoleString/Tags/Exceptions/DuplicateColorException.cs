using Gravy.Ansi;

namespace Gravy.ConsoleString.Tags;

public class DuplicateColorException : TagsCompilerException
{
    public string Type { get; set; }
    public AnsiColor Value { get; set; }

    public DuplicateColorException(string type, AnsiColor value, int line, int column) : base(line, column)
    {
        Type = type;
        Value = value;
    }

    public override string Message => $"Color {Value} for {Type} was already set at line {Line}:{Column}";
}