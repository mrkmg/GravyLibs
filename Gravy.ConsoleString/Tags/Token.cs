using System;
using System.Drawing;

namespace Gravy.ConsoleString.Tags;

internal class Token
{
    private readonly object? _value;
    public int Line { get; }
    public int Column { get; }

    private Token(object? value, int line, int column)
    {
        _value = value;
        Line = line;
        Column = column;
    }
    
    public static TextToken Text(string value, int line, int column) => new(value, line, column);
    public static ForegroundStartToken ForegroundStart(Color value, int line, int column) => new(value, line, column);
    public static BackgroundStartToken BackgroundStart(Color value, int line, int column) => new(value, line, column);
    public static ForegroundStopToken ForegroundStop(int line, int column) => new(line, column);
    public static BackgroundStopToken BackgroundStop(int line, int column) => new(line, column);
    public static StyleStartToken StyleStart(FontStyle value, int line, int column) => new(value, line, column);
    public static StyleStopToken StyleStop(FontStyle value, int line, int column) => new(value, line, column);
    public static WeightStartToken WeightStart(FontWeight value, int line, int column) => new(value, line, column);
    public static WeightStopToken WeightStop(FontWeight value, int line, int column) => new(value, line, column);
    public static ResetAllToken ResetAll(int line, int column) => new(line, column);
    
    public class TextToken : Token
    {
        public string Value => (string)(_value ?? throw new InvalidOperationException($"TextToken should have a value"));
        public TextToken(string value, int line, int column) : base(value, line, column) { }
    }
    public class ForegroundStartToken : Token
    {
        public Color Value => (Color)(_value ?? throw new InvalidOperationException($"ForegroundStartToken should have a value"));
        public ForegroundStartToken(Color color, int line, int column) : base(color, line, column) { }
    }
    public class BackgroundStartToken : Token
    {
        public Color Value => (Color)(_value ?? throw new InvalidOperationException($"BackgroundStartToken should have a value"));
        public BackgroundStartToken(Color color, int line, int column) : base(color, line, column) { }
    }
    public class ForegroundStopToken : Token
    {
        public ForegroundStopToken(int line, int column) : base(null, line, column) { }
    }
    public class BackgroundStopToken : Token
    {
        public BackgroundStopToken(int line, int column) : base(null, line, column) { }
    }
    public class StyleStartToken : Token
    {
        public FontStyle Value => (FontStyle)(_value ?? throw new InvalidOperationException($"StyleStartToken should have a value"));
        public StyleStartToken(FontStyle style, int line, int column) : base(style, line, column) { }
    }
    public class StyleStopToken : Token
    {
        public FontStyle Value => (FontStyle)(_value ?? throw new InvalidOperationException($"StyleStopToken should have a value"));
        public StyleStopToken(FontStyle style, int line, int column) : base(style, line, column) { }
    }
    public class WeightStartToken : Token
    {
        public FontWeight Value => (FontWeight)(_value ?? throw new InvalidOperationException($"WeightStartToken should have a value"));
        public WeightStartToken(FontWeight weight, int line, int column) : base(weight, line, column) { }
    }
    public class WeightStopToken : Token
    {
        public FontWeight Value => (FontWeight)(_value ?? throw new InvalidOperationException($"WeightStopToken should have a value"));
        public WeightStopToken(FontWeight weight, int line, int column) : base(weight, line, column) { }
    }
    public class ResetAllToken : Token
    {
        public ResetAllToken(int line, int column) : base(null, line, column) { }
    }
}