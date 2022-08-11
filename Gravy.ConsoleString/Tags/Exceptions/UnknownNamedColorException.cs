namespace Gravy.ConsoleString.Tags;

public class UnknownNamedColorException : TagsCompilerException
{
    public string ColorName { get; }

    public UnknownNamedColorException(string colorName, int line, int column) : base(line, column)
    {
        ColorName = colorName;
    }

    public override string Message => $"The color name '{ColorName}' is not known at index {Line}:{Column}";
}