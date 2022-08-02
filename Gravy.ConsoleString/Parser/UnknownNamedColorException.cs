namespace Gravy.ConsoleString.Parser;

public class UnknownNamedColorException : ParserException
{
    public string ColorName { get; }

    public UnknownNamedColorException(string colorName, int index)
    {
        ColorName = colorName;
        Index = index;
    }

    public override string Message => $"The color name '{ColorName}' is not known at index {Index}";
}