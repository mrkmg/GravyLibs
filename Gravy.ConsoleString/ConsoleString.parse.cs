using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Gravy.ConsoleString.Parser;

namespace Gravy.ConsoleString;

public partial class ConsoleString
{
    /// <summary>
    /// Parses a string with ConsoleStringTags into a ConsoleString
    /// </summary>
    /// <param name="str">The input formatted string.</param>
    /// <returns>The parsed ConsoleString</returns>
    /// <exception cref="Exception">Parser Errors</exception>
    /// <remarks>
    /// <para>Tags:</para>
    /// <b>[F#XXXXXX]</b> Set foreground color to Hex color 'XXXXXX'.<br />
    /// <b>[F!Name]</b>   Set foreground color to a known color named 'Name'. See <see cref="KnownColor"/><br />
    /// <b>[/F]</b>       Reset foreground color to default.<br />
    /// <b>[G#XXXXXX]</b> Set background color to Hex color XXXXXX.<br />
    /// <b>[G!Name]</b>   Set background color to Hex color named 'Name'. See <see cref="KnownColor"/><br />
    /// <b>[/G]</b>       Reset background color to default.<br />
    /// <b>[B]</b>        Enable Bold.<br />
    /// <b>[/B]</b>       Disable Bold.<br />
    /// <b>[I]</b>        Enable Italic.<br />
    /// <b>[/I]</b>       Disable Italic.<br />
    /// <b>[U]</b>        Enable Underline.<br />
    /// <b>[/U]</b>       Disable Underline.<br />
    /// </remarks>
    public static ConsoleString Parse(string str)
        => new ConsoleStringTagsParser(str).Parse();
}