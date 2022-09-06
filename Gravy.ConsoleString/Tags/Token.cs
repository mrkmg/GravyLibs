using System.Drawing;

namespace Gravy.ConsoleString.Tags;

// ReSharper disable once InconsistentNaming
internal interface Token
{
    public int Line { get; }
    public int Column { get; }
    
    public static TextToken Text(string text, int line, int column) => new(text, line, column);
    public static ForegroundStartToken ForegroundStart(AnsiColor color, int line, int column) => new(color, line, column);
    public static BackgroundStartToken BackgroundStart(AnsiColor color, int line, int column) => new(color, line, column);
    public static ForegroundStopToken ForegroundStop(int line, int column) => new(line, column);
    public static BackgroundStopToken BackgroundStop(int line, int column) => new(line, column);
    public static StyleStartToken StyleStart(FontStyle fontStyle, int line, int column) => new(fontStyle, line, column);
    public static StyleStopToken StyleStop(FontStyle fontStyle, int line, int column) => new(fontStyle, line, column);
    public static WeightStartToken WeightStart(FontWeight fontWeight, int line, int column) => new(fontWeight, line, column);
    public static WeightStopToken WeightStop(FontWeight fontWeight, int line, int column) => new(fontWeight, line, column);
    public static ResetAllToken ResetAll(int line, int column) => new(line, column);
    
    public struct TextToken : Token
    {
        public int Line { get; }
        public int Column { get; }
        public string Text { get; }

        public TextToken(string text, int line, int column)
        {
            Text = text;
            Line = line;
            Column = column;
        }
    }
    
    public struct ForegroundStartToken : Token
    {
        public int Line { get; }
        public int Column { get; }
        public AnsiColor Color { get; }

        public ForegroundStartToken(AnsiColor color, int line, int column)
        {
            Color = color;
            Line = line;
            Column = column;
        }
    }
    
    public struct BackgroundStartToken : Token
    {
        public int Line { get; }
        public int Column { get; }
        public AnsiColor Color { get; }

        public BackgroundStartToken(AnsiColor color, int line, int column)
        {
            Color = color;
            Line = line;
            Column = column;
        }
    }
    
    public struct ForegroundStopToken : Token
    {
        public int Line { get; }
        public int Column { get; }

        public ForegroundStopToken(int line, int column)
        {
            Line = line;
            Column = column;
        }
    }
    
    public struct BackgroundStopToken : Token
    {
        public int Line { get; }
        public int Column { get; }

        public BackgroundStopToken(int line, int column)
        {
            Line = line;
            Column = column;
        }
    }
    
    public struct StyleStartToken : Token
    {
        public int Line { get; }
        public int Column { get; }
        public FontStyle FontStyle { get; }

        public StyleStartToken(FontStyle style, int line, int column)
        {
            FontStyle = style;
            Line = line;
            Column = column;
        }
    }
    
    public struct StyleStopToken : Token
    {
        public int Line { get; }
        public int Column { get; }
        public FontStyle FontStyle { get; }

        public StyleStopToken(FontStyle style, int line, int column)
        {
            FontStyle = style;
            Line = line;
            Column = column;
        }
    }
    
    public struct WeightStartToken : Token
    {
        public int Line { get; }
        public int Column { get; }
        public FontWeight FontWeight { get; }
        
        public WeightStartToken(FontWeight weight, int line, int column)
        {
            FontWeight = weight;
            Line = line;
            Column = column;
        }
    }
    
    public struct WeightStopToken : Token
    {
        public int Line { get; }
        public int Column { get; }
        public FontWeight FontWeight { get; }
        
        public WeightStopToken(FontWeight weight, int line, int column)
        {
            FontWeight = weight;
            Line = line;
            Column = column;
        }
    }
    
    public struct ResetAllToken : Token
    {
        public int Line { get; }
        public int Column { get; }

        public ResetAllToken(int line, int column)
        {
            Line = line;
            Column = column;
        }
    }
}