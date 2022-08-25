using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Gravy.ConsoleString;

[DebuggerDisplay("{DebugDisplay()}")]
public readonly struct ConsoleFormat
{
    public static readonly ConsoleFormat Default = new();
    
    public readonly Color? ForegroundColor;
    public readonly Color? BackgroundColor;
    public readonly FontWeight Weight;
    public readonly FontStyle Styles;
    
    public ConsoleFormat(Color? foreground = null, Color? background = null, FontWeight weight = FontWeight.Normal, FontStyle styles = FontStyle.None)
    {
        ForegroundColor = foreground;
        BackgroundColor = background;
        Weight = weight;
        Styles = styles;
    }
    
    public ConsoleFormat WithForeground(Color? foreground) => new(foreground, BackgroundColor, Weight, Styles);
    public ConsoleFormat WithBackground(Color? background) => new(ForegroundColor, background, Weight, Styles);
    
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
        => MetaStringConsoleFormat.AreColorsEquivalent(ForegroundColor, other.ForegroundColor) && 
           MetaStringConsoleFormat.AreColorsEquivalent(BackgroundColor, other.BackgroundColor) && 
           Weight == other.Weight && Styles == other.Styles;

    public override bool Equals(object? obj)
        => obj is ConsoleFormat other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(ForegroundColor, BackgroundColor, (int)Weight, (int)Styles);

    public override string ToString()
        => DebugDisplay();
    
    [DebuggerHidden]
    private string DebugDisplay()
    {
        var attributes = new List<string>();
        if (ForegroundColor is {} f) attributes.Add("FG=" + f.ToCsColor());
        if (BackgroundColor is {} b) attributes.Add("BG=" + b.ToCsColor());
        if (Weight != FontWeight.Normal) attributes.Add("W=" + Weight);
        if (Styles.HasFlag(FontStyle.Underline)) attributes.Add("U");
        if (Styles.HasFlag(FontStyle.Italic)) attributes.Add("I");
        if (Styles.HasFlag(FontStyle.StrikeThrough)) attributes.Add("S");
        if (Styles.HasFlag(FontStyle.Blink)) attributes.Add("L");
        if (Styles.HasFlag(FontStyle.Inverse)) attributes.Add("V");
        return "ConsoleFormat(" + string.Join(", ", attributes) + ")";
    }
}