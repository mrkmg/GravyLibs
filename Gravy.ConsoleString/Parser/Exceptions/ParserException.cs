using System;

namespace Gravy.ConsoleString.Parser;

public abstract class ParserException : Exception
{
    public int Index { get; protected set; }
}