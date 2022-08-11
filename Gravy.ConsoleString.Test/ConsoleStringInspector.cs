using System.Drawing;

namespace Gravy.ConsoleString.Test;

public static class ConsoleStringInspector
{
    internal static IEnumerable<Color?> ForegroundColors(this ConsoleString cs)
        => cs.MetaData.Select(e => e.Data.ForegroundColor);
    
    internal static IEnumerable<Color?> BackgroundColors(this ConsoleString cs)
        => cs.MetaData.Select(e => e.Data.BackgroundColor);
    
    internal static IEnumerable<FontWeight> Weights(this ConsoleString cs)
        => cs.MetaData.Select(e => e.Data.Weight);
    
    internal static IEnumerable<FontStyle> Styles(this ConsoleString cs)
        => cs.MetaData.Select(e => e.Data.Styles);
}