using System;
using System.Drawing;
using Gravy.Ansi;
using Gravy.MetaString;

namespace Gravy.ConsoleString;

using ConsoleString = MetaString<ConsoleFormat>;

public static class ConsoleStringExtensions
{
    public static ConsoleString ToConsoleString(this string str)
        => new(str);
    
    // ReSharper disable once InconsistentNaming
    public static ConsoleString ParseCS(this string str)
        => str.FromTags();
    
    public static ConsoleString With(this string str, ConsoleFormat format)
        => new(str, format);
    
    public static ConsoleString With(this string str, FontWeight weight)
        => str.With(null, null, weight);
    
    public static ConsoleString With(this string str, FontStyle style)
        => str.With(null, null, default, style);
    
    public static ConsoleString With(this string str, AnsiColor? foreground = null, AnsiColor? background = null, FontWeight weight = FontWeight.Normal, FontStyle styles = FontStyle.None)
        => new(str, new(foreground, background, weight, styles));
    
    internal static string ToHex(this Color color)
        => color.A == 0xFF ? (color.ToArgb() & 0x00FFFFFF).ToString("X6") : color.ToArgb().ToString("X8");
    

    public static string ToCsColor(this AnsiColor color)
    {
        return color.Type switch {
            AnsiColorType.Ansi16 => "@" + color.Ansi16Color,
            AnsiColorType.Ansi256 => "$" + color.Ansi256Color,
            AnsiColorType.Rgb when color.RgbColor.IsNamedColor => "!" + color.RgbColor.Name,
            AnsiColorType.Rgb => "#" + color.RgbColor.ToHex(),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }


}