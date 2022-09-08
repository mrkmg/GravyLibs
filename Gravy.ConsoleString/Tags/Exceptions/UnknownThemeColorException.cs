namespace Gravy.ConsoleString.Tags;

public class UnknownThemeColorException : TagsCompilerException
{
    public string Detail { get; }
    public string ThemeValue { get; }

    public UnknownThemeColorException(string detail, string themeValue, int line, int column) : base(line, column)
    {
        Detail = detail;
        ThemeValue = themeValue;
    }

    public override string Message => $"Invalid Ansi16 Color '{ThemeValue}' at {Line}:{Column} ({Detail})";
}