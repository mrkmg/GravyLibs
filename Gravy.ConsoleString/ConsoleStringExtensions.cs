using System.Drawing;
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
    
    internal static bool IsEquivalent(this Color color1, Color color2)
        => color1.ToArgb() == color2.ToArgb(); 

    public static string ToCsColor(this AnsiColor color)
    {
        if (color.IsThemeColor) return "@" + color.ThemeColor;
        if (color.SystemColor.IsNamedColor) return "!" + color.SystemColor.Name;
        return "#" + color.SystemColor.ToHex();
    }


}