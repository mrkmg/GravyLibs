namespace Gravy.ConsoleString.Tags;

internal class InvalidHexColorException : TagsCompilerException
{
    public string HexValue { get; }

    public InvalidHexColorException(string hexValue, int line, int column) : base(line, column)
    {
        HexValue = hexValue;
    }

    public override string Message => $"Invalid Hex Color '{HexValue}' at {Line}:{Column}";
}