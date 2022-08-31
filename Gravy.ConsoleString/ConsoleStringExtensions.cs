using System.Drawing;
using Gravy.MetaString;

namespace Gravy.ConsoleString;

using ConsoleString = MetaString<ConsoleFormat>;

public static class ConsoleStringExtensions
{
    public static ConsoleString ToConsoleString(this string str)
        => new(str);
    
    // ReSharper disable once InconsistentNaming
    public static ConsoleString CS(this string str)
        => str.FromTags();

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
    
    internal static bool IsEquivalent(this Color? color1, Color? color2)
        => (color1 is null && color2 is null) || (color1 is not null && color2 is not null && color1.Value.ToArgb() == color2.Value.ToArgb()); 

    public static string ToCsColor(this Color color)
    {
        if (color.IsNamedColor) return "!" + color.Name;
        return "#" + color.ToHex();
    }


}