using System.Drawing;
using Gravy.MetaString;

namespace Gravy.ConsoleString.Test;

using ConsoleString = MetaString<ConsoleFormat>;

public static class ConsoleStringInspector
{
    internal static IEnumerable<AnsiColor?> ForegroundColors(this ConsoleString cs)
        => cs.MetaData.Select(e => e.Data.ForegroundColor);
    
    internal static IEnumerable<AnsiColor?> BackgroundColors(this ConsoleString cs)
        => cs.MetaData.Select(e => e.Data.BackgroundColor);
    
    internal static IEnumerable<FontWeight> Weights(this ConsoleString cs)
        => cs.MetaData.Select(e => e.Data.Weight);
    
    internal static IEnumerable<FontStyle> Styles(this ConsoleString cs)
        => cs.MetaData.Select(e => e.Data.Styles);
}