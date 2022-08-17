using System.Drawing;
using Ansi;

namespace Gravy.ConsoleString;

public static class ConsoleStringExtensions
{
    public static ConsoleString ToConsoleString(this string str)
        => new(str);
    
    // ReSharper disable once InconsistentNaming
    public static ConsoleString CS(this string str)
        => ConsoleString.Parse(str);

    // ReSharper disable once InconsistentNaming
    public static ConsoleString FG(this ConsoleString cStr, Color color)
        => cStr.WithForeground(color);
    
    // ReSharper disable once InconsistentNaming
    public static ConsoleString BG(this ConsoleString cStr, Color color)
        => cStr.WithBackground(color);
    
    public static ConsoleString With(this string str, ConsoleFormat format)
        => new(str, format);

    internal static AnsiColor ToAnsi(this Color color)
        => new(color.R, color.G, color.B);
    
    internal static string ToHex(this Color color)
        => color.A == 0 ? color.ToArgb().ToString("X6") : color.ToArgb().ToString("X8");

    public static string ToCsColor(this Color color)
    {
        if (color.IsNamedColor) return "!" + color.Name;
        return "#" + color.ToHex();
    }


}