using System;
using Gravy.Ansi;
using Gravy.MetaString;

namespace Gravy.ConsoleString;

public readonly struct ConsoleFormat : IMetaDebuggable<ConsoleFormat>
{
    public static readonly ConsoleFormat Default = new();
    
    public readonly AnsiColor? ForegroundColor;
    public readonly AnsiColor? BackgroundColor;
    public readonly FontWeight Weight;
    public readonly FontStyle Styles;
    
    public ConsoleFormat(AnsiColor? foreground = null, AnsiColor? background = null, FontWeight weight = FontWeight.Normal, FontStyle styles = FontStyle.None)
    {
        ForegroundColor = foreground;
        BackgroundColor = background;
        Weight = weight;
        Styles = styles;
    }
    
    public ConsoleFormat(FontWeight weight) : this(null, null, weight) { }
    public ConsoleFormat(FontStyle styles) : this(null, null, FontWeight.Normal, styles) { }
    public ConsoleFormat(FontWeight weight, FontStyle styles) : this(null, null, weight, styles) { }

    public ConsoleFormat WithForeground(AnsiColor? foreground) => new(foreground, BackgroundColor, Weight, Styles);
    public ConsoleFormat WithBackground(AnsiColor? background) => new(ForegroundColor, background, Weight, Styles);
    
    public ConsoleFormat WithWeight(FontWeight weight)
        => new(ForegroundColor, BackgroundColor, weight, Styles);
    public ConsoleFormat WithStyle(FontStyle style)
        => new(ForegroundColor, BackgroundColor, Weight, Styles | style);
    public ConsoleFormat WithoutStyle(FontStyle style)
        => new(ForegroundColor, BackgroundColor, Weight, Styles & ~style);
    public ConsoleFormat ResetStyle()
        => new(ForegroundColor, BackgroundColor, Weight);
    
    public ConsoleFormat WithNormal() => WithWeight(FontWeight.Normal);
    public ConsoleFormat WithBold() => WithWeight(FontWeight.Bold);
    public ConsoleFormat WithLight() => WithWeight(FontWeight.Light);
    
    public ConsoleFormat WithItalic() => WithStyle(FontStyle.Italic);
    public ConsoleFormat WithoutItalic() => WithoutStyle(FontStyle.Italic);
    
    public ConsoleFormat WithUnderline() => WithStyle(FontStyle.Underline);
    public ConsoleFormat WithoutUnderline() => WithoutStyle(FontStyle.Underline);
    
    public ConsoleFormat WithBlink() => WithStyle(FontStyle.Blink);
    public ConsoleFormat WithoutBlink() => WithoutStyle(FontStyle.Blink);
    
    public ConsoleFormat WithInverse() => WithStyle(FontStyle.Inverse);
    public ConsoleFormat WithoutInverse() => WithoutStyle(FontStyle.Inverse);
    
    public ConsoleFormat WithStrikeThrough() => WithStyle(FontStyle.StrikeThrough);
    public ConsoleFormat WithoutStrikeThrough() => WithoutStyle(FontStyle.StrikeThrough);
    
    public bool Equals(ConsoleFormat other) 
        => ForegroundColor == other.ForegroundColor && 
           BackgroundColor == other.BackgroundColor && 
           Weight == other.Weight && Styles == other.Styles;

    public string DebugString(MetaString<ConsoleFormat> metaString) => metaString.ToTaggedString(true);

    public override bool Equals(object? obj)
        => obj is ConsoleFormat other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(ForegroundColor, BackgroundColor, (int)Weight, (int)Styles);
    
}

[Flags]
public enum FontStyle
{
    None = 0,
    Italic = 1,
    Underline = 2,
    Blink = 8,
    Inverse = 16,
    StrikeThrough = 32,
}

public enum FontWeight
{
    Normal,
    Bold,
    Light,
}