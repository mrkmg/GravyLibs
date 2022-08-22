using System;
using System.Drawing;
using Gravy.ConsoleString.Tags;

namespace Gravy.ConsoleString;

public partial class ConsoleString
{
    /// <summary>
    /// Parses a string with ConsoleStringTags into a ConsoleString
    /// </summary>
    /// <param name="str">The input formatted string.</param>
    /// <param name="strictMode">Enable on strict mode</param>
    /// <returns>The parsed ConsoleString</returns>
    /// <exception cref="Exception">Parser Errors</exception>
    /// <remarks>
    /// <para>Tags:</para>
    /// <b>[F#XXXXXX]</b> Set foreground color to Hex color 'XXXXXX'.<br />
    /// <b>[F!Name]</b>   Set foreground color to a known color named 'Name'. See <see cref="KnownColor"/><br />
    /// <b>[/F]</b>       Stop the current foreground color.<br />
    /// <b>[G#XXXXXX]</b> Set background color to Hex color XXXXXX.<br />
    /// <b>[G!Name]</b>   Set background color to Hex color named 'Name'. See <see cref="KnownColor"/><br />
    /// <b>[/G]</b>       Stop the current background color.<br />
    /// <b>[B]</b>        Enable Bold.<br />
    /// <b>[/B]</b>       Disable Bold.<br />
    /// <b>[L]</b>        Enable Light.<br />
    /// <b>[/L]</b>       Disable Light.<br />
    /// <b>[I]</b>        Enable Italic.<br />
    /// <b>[/I]</b>       Disable Italic.<br />
    /// <b>[U]</b>        Enable Underline.<br />
    /// <b>[/U]</b>       Disable Underline.<br />
    /// <b>[T]</b>        Enable Strikethrough.<br />
    /// <b>[/T]</b>       Disable Strikethrough.<br />
    /// </remarks>
    public static ConsoleString Parse(string str, bool strictMode = false)
        => Assembler.Assemble(Tokenizer.Tokenize(str), strictMode);

    public static string EscapeTags(string str) => string.Join(@"\[", str.Split('['));
}