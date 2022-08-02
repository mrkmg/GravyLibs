namespace Gravy.ConsoleString.Parser;

internal class InvalidHexColorException : ParserException
{
    public string HexValue { get; }

    public InvalidHexColorException(string hexValue, int index)
    {
        HexValue = hexValue;
        Index = index;
    }

    public override string Message => $"Invalid Hex Color '{HexValue}' at {Index}";
}