using System;

namespace Gravy.ConsoleString.Tags;

public abstract class TagsCompilerException : Exception
{
    public int Line { get; }
    public int Column { get; }
    
    public TagsCompilerException(int line, int column)
    {
        Line = line;
        Column = column;
    }
}