using System;
using System.Drawing;

namespace Gravy.ConsoleString;

public readonly struct ConsoleFormat
{
    public static readonly ConsoleFormat Default = new();
    
    public readonly Color? ForegroundColor;
    public readonly Color? BackgroundColor;
    public readonly FontStyle Styles;
    
    
    public ConsoleFormat(Color? foreground = null, Color? background = null, FontStyle styles = FontStyle.None)
    {
        ForegroundColor = foreground;
        BackgroundColor = background;
        Styles = styles;
    }
    
    public ConsoleFormat WithForeground(Color? foreground) => new(foreground, BackgroundColor, Styles);
    public ConsoleFormat WithBackground(Color? background) => new(ForegroundColor, background, Styles);
    
    public ConsoleFormat WithStyle(FontStyle style)
        => new(ForegroundColor, BackgroundColor, Styles | style);
    public ConsoleFormat WithoutStyle(FontStyle style)
        => new(ForegroundColor, BackgroundColor, Styles & ~style);
    public ConsoleFormat ResetStyle()
        => new(ForegroundColor, BackgroundColor);
    
    public ConsoleFormat WithBold() => WithStyle(FontStyle.Bold);
    public ConsoleFormat WithItalic() => WithStyle(FontStyle.Italic);
    public ConsoleFormat WithUnderline() => WithStyle(FontStyle.Underline);
    public ConsoleFormat WithoutBold() => WithoutStyle(FontStyle.Bold);
    public ConsoleFormat WithoutItalic() => WithoutStyle(FontStyle.Italic);
    public ConsoleFormat WithoutUnderline() => WithoutStyle(FontStyle.Underline);
    
    public bool Equals(ConsoleFormat other)
    {
        if (ForegroundColor.HasValue && !other.ForegroundColor.HasValue || !ForegroundColor.HasValue && other.ForegroundColor.HasValue)
            return false;
        if (ForegroundColor.HasValue && ForegroundColor.Value.ToArgb() != other.ForegroundColor!.Value.ToArgb())
            return false;
        
        if (BackgroundColor.HasValue && !other.BackgroundColor.HasValue || !BackgroundColor.HasValue && other.BackgroundColor.HasValue)
            return false;
        if (BackgroundColor.HasValue && BackgroundColor.Value.ToArgb() != other.BackgroundColor!.Value.ToArgb())
            return false;

        return Styles == other.Styles;
    }

    public override bool Equals(object? obj)
        => obj is ConsoleFormat other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(ForegroundColor, BackgroundColor, (int)Styles);
}