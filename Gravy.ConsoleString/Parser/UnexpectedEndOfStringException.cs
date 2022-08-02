namespace Gravy.ConsoleString.Parser;

public class UnexpectedEndOfStringException : ParserException
{
    public UnexpectedEndOfStringException(int index)
    {
        Index = index;
    }
    
    public override string Message => $"Unexpected end of string at when parsing token at {Index}";
}