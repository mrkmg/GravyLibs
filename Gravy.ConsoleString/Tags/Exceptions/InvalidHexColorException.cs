namespace Gravy.ConsoleString.Tags;

internal class InvalidHexColorException : TagsCompilerException
{
    public string Detail { get; }
    public string HexValue { get; }

    public InvalidHexColorException(string detail, string hexValue, int line, int column) : base(line, column)
    {
        Detail = detail;
        HexValue = hexValue;
    }

    public override string Message => $"Invalid Hex Color '{HexValue}' at {Line}:{Column} ({Detail})";
}