using System.Collections;
using System.Data;
using System.Drawing;
using System.Reflection;
using Gravy.MetaString;

namespace Gravy.ConsoleString.Test;

public static class ConsoleStringInspector
{
    internal static IEnumerable<Color?> ForegroundColors(this ConsoleString cs)
        => cs.MetaEntries.Select(e => e.Data.ForegroundColor);
    
    internal static IEnumerable<Color?> BackgroundColors(this ConsoleString cs)
        => cs.MetaEntries.Select(e => e.Data.BackgroundColor);
    
    internal static IEnumerable<FontStyle> Styles(this ConsoleString cs)
        => cs.MetaEntries.Select(e => e.Data.Styles);
}