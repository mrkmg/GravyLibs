using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Ansi;
using Gravy.ConsoleString.Tags;
using Gravy.MetaString;


namespace Gravy.ConsoleString;

using ConsoleString = Gravy.MetaString.MetaString<Gravy.ConsoleString.ConsoleFormat>;

public static class MetaStringConsoleFormat
{
    public static ConsoleString From(this string str, ConsoleFormat format) => new (str, format);
    public static ConsoleString With(this ConsoleString cs, ConsoleFormat format) => new (cs.RawText, format);
    public static ConsoleString WithForeground(this ConsoleString cs, Color? color) 
        => new (cs.MetaData.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithForeground(color))));
    public static ConsoleString WithBackground(this ConsoleString cs, Color? color) 
        => new (cs.MetaData.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithBackground(color))));
    public static ConsoleString WithStyle(this ConsoleString cs, FontStyle style) 
        => new (cs.MetaData.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithStyle(style))));
    public static ConsoleString WithoutStyle(this ConsoleString cs, FontStyle style) 
        => new (cs.MetaData.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithoutStyle(style))));
    public static ConsoleString WithWeight(this ConsoleString cs, FontWeight weight) 
        => new (cs.MetaData.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithWeight(weight))));
    public static ConsoleString ResetStyle(this ConsoleString cs) 
        => new (cs.MetaData.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.ResetStyle())));
    public static ConsoleString WithNormal(this ConsoleString cs) => cs.WithWeight(FontWeight.Normal);
    public static ConsoleString WithBold(this ConsoleString cs) => cs.WithWeight(FontWeight.Bold);
    public static ConsoleString WithLight(this ConsoleString cs) => cs.WithWeight(FontWeight.Light);
    public static ConsoleString WithItalic(this ConsoleString cs) => cs.WithStyle(FontStyle.Italic);
    public static ConsoleString WithoutItalic(this ConsoleString cs) => cs.WithoutStyle(FontStyle.Italic);
    public static ConsoleString WithUnderline(this ConsoleString cs) => cs.WithStyle(FontStyle.Underline);
    public static ConsoleString WithoutUnderline(this ConsoleString cs) => cs.WithoutStyle(FontStyle.Underline);
    public static ConsoleString WithBlink(this ConsoleString cs) => cs.WithStyle(FontStyle.Blink);
    public static ConsoleString WithoutBlink(this ConsoleString cs) => cs.WithoutStyle(FontStyle.Blink);
    public static ConsoleString WithInverse(this ConsoleString cs) => cs.WithStyle(FontStyle.Inverse);
    public static ConsoleString WithoutInverse(this ConsoleString cs) => cs.WithoutStyle(FontStyle.Inverse);
    public static ConsoleString WithStrikeThrough(this ConsoleString cs) => cs.WithStyle(FontStyle.StrikeThrough);
    public static ConsoleString WithoutStrikeThrough(this ConsoleString cs) => cs.WithoutStyle(FontStyle.StrikeThrough);
    
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
    public static ConsoleString FromTags(this string str, bool strictMode = false) 
        => Assembler.Assemble(Tokenizer.Tokenize(str), strictMode).Optimize();
    
    public static string EscapeTags(this string str) => string.Join(@"\[", str.Split('['));
    
    public static string ToAnsiString(this ConsoleString cs)
    {
        if (!cs.MetaData.Any())
            return string.Empty;

        void WriteFormatDiff(StringBuilder sb, ConsoleFormat fromFormat, ConsoleFormat toFormat)
        {
            if (toFormat.Equals(ConsoleFormat.Default) && !fromFormat.Equals(ConsoleFormat.Default))
            {
                sb.SetMode(Mode.Reset);
                return;
            }

            if (!AreColorsEquivalent(toFormat.BackgroundColor, fromFormat.BackgroundColor))
            {
                if (toFormat.BackgroundColor.HasValue)
                    sb.SetBackgroundColor(toFormat.BackgroundColor.Value.ToAnsi());
                else
                    sb.SetMode(Mode.BackgroundDefault);
            }
            
            if (!AreColorsEquivalent(toFormat.ForegroundColor, fromFormat.ForegroundColor))
            {
                if (toFormat.ForegroundColor.HasValue)
                    sb.SetForegroundColor(toFormat.ForegroundColor.Value.ToAnsi());
                else
                    sb.SetMode(Mode.ForegroundDefault);
            }
            
            switch (toFormat.Weight)
            {
                case FontWeight.Normal when fromFormat.Weight != FontWeight.Normal:
                    sb.SetMode(Mode.Normal);
                    break;
                case FontWeight.Bold when fromFormat.Weight != FontWeight.Bold:
                    sb.SetMode(Mode.Bold);
                    break;
                case FontWeight.Light when fromFormat.Weight != FontWeight.Light:
                    sb.SetMode(Mode.Faint);
                    break;
            }

            var modes = new List<int>();

            if (!fromFormat.Styles.HasFlag(FontStyle.Italic) && toFormat.Styles.HasFlag(FontStyle.Italic))
                modes.Add((int)Mode.Italic);
            if (fromFormat.Styles.HasFlag(FontStyle.Italic) && !toFormat.Styles.HasFlag(FontStyle.Italic))
                modes.Add((int)Mode.NoItalic);
                
            if (!fromFormat.Styles.HasFlag(FontStyle.Underline) && toFormat.Styles.HasFlag(FontStyle.Underline))
                modes.Add((int)Mode.Underline);
            if (fromFormat.Styles.HasFlag(FontStyle.Underline) && !toFormat.Styles.HasFlag(FontStyle.Underline))
                modes.Add((int)Mode.NoUnderline);
                
            if (!fromFormat.Styles.HasFlag(FontStyle.Blink) && toFormat.Styles.HasFlag(FontStyle.Blink))
                modes.Add((int)Mode.Blink);
            if (fromFormat.Styles.HasFlag(FontStyle.Blink) && !toFormat.Styles.HasFlag(FontStyle.Blink))
                modes.Add((int)Mode.NoBlink);
                
            if (!fromFormat.Styles.HasFlag(FontStyle.Inverse) && toFormat.Styles.HasFlag(FontStyle.Inverse))
                modes.Add((int)Mode.Inverse);
            if (fromFormat.Styles.HasFlag(FontStyle.Inverse) && !toFormat.Styles.HasFlag(FontStyle.Inverse))
                modes.Add((int)Mode.NoInverse);
                
            if (!fromFormat.Styles.HasFlag(FontStyle.StrikeThrough) && toFormat.Styles.HasFlag(FontStyle.StrikeThrough))
                modes.Add(9); // ANSI package does not have a strike through mode
            if (fromFormat.Styles.HasFlag(FontStyle.StrikeThrough) && !toFormat.Styles.HasFlag(FontStyle.StrikeThrough))
                modes.Add(29); // ANSI package does not have a strike through mode

            if (modes.Count > 0)
                sb.SetMode(modes.Select(x => (Mode)x).ToArray());
        } 
        
        var currentFormat = ConsoleFormat.Default;
        var sb = new StringBuilder();
        foreach (var entry in cs.MetaData)
        {
            WriteFormatDiff(sb, currentFormat, entry.Data);
            sb.Append(entry.Text);
            currentFormat = entry.Data;
        }
        WriteFormatDiff(sb, currentFormat, ConsoleFormat.Default);
        return sb.ToString();
    }
    
    public static string ToTaggedString(this ConsoleString cs, bool useReset = false)
    {
        if (!cs.MetaData.Any())
            return string.Empty;

        void WriteFormatDiff(StringBuilder sb, ConsoleFormat fromFormat, ConsoleFormat toFormat)
        {
            if (useReset && toFormat.Equals(ConsoleFormat.Default) && !fromFormat.Equals(ConsoleFormat.Default))
            {
                sb.Append("[//]");
                return;
            }
            
            if (toFormat.BackgroundColor != fromFormat.BackgroundColor)
            {
                if (toFormat.BackgroundColor.HasValue)
                    sb.Append($@"[G{toFormat.BackgroundColor.Value.ToCsColor()}]");
                else
                    sb.Append(@"[/G]");
            }
            if (toFormat.ForegroundColor != fromFormat.ForegroundColor)
            {
                if (toFormat.ForegroundColor.HasValue)
                    sb.Append($@"[F{toFormat.ForegroundColor.Value.ToCsColor()}]");
                else
                    sb.Append(@"[/F]");
            }
            
            if (fromFormat.Weight != toFormat.Weight)
            {
                if (fromFormat.Weight == FontWeight.Bold)
                    sb.Append(@"[/B]");
                if (fromFormat.Weight == FontWeight.Light)
                    sb.Append(@"[/L]");
                
                if (toFormat.Weight == FontWeight.Bold)
                    sb.Append(@"[B]");
                if (toFormat.Weight == FontWeight.Light)
                    sb.Append(@"[L]");
            }

            if (!fromFormat.Styles.HasFlag(FontStyle.Italic) && toFormat.Styles.HasFlag(FontStyle.Italic))
                sb.Append("[I]");
            if (fromFormat.Styles.HasFlag(FontStyle.Italic) && !toFormat.Styles.HasFlag(FontStyle.Italic))
                sb.Append("[/I]");
                
            if (!fromFormat.Styles.HasFlag(FontStyle.Underline) && toFormat.Styles.HasFlag(FontStyle.Underline))
                sb.Append("[U]");
            if (fromFormat.Styles.HasFlag(FontStyle.Underline) && !toFormat.Styles.HasFlag(FontStyle.Underline))
                sb.Append("[/U]");
                
            if (!fromFormat.Styles.HasFlag(FontStyle.Blink) && toFormat.Styles.HasFlag(FontStyle.Blink))
                sb.Append("[K]");
            if (fromFormat.Styles.HasFlag(FontStyle.Blink) && !toFormat.Styles.HasFlag(FontStyle.Blink))
                sb.Append("[/K]");
                
            if (!fromFormat.Styles.HasFlag(FontStyle.Inverse) && toFormat.Styles.HasFlag(FontStyle.Inverse))
                sb.Append("[V]");
            if (fromFormat.Styles.HasFlag(FontStyle.Inverse) && !toFormat.Styles.HasFlag(FontStyle.Inverse))
                sb.Append("[/V]");
                
            if (!fromFormat.Styles.HasFlag(FontStyle.StrikeThrough) && toFormat.Styles.HasFlag(FontStyle.StrikeThrough))
                sb.Append("[T]");
            if (fromFormat.Styles.HasFlag(FontStyle.StrikeThrough) && !toFormat.Styles.HasFlag(FontStyle.StrikeThrough))
                sb.Append("[/T]");
        }
        
        var currentFormat = ConsoleFormat.Default;
        var sb = new StringBuilder();
        foreach (var entry in cs.MetaData)
        {
            WriteFormatDiff(sb, currentFormat, entry.Data);
            sb.Append(entry.Text);
            currentFormat = entry.Data;
        }
        WriteFormatDiff(sb, currentFormat, ConsoleFormat.Default);
        return sb.ToString();
    }
    
    public static ConsoleString Optimize(this ConsoleString cs) => new(cs.MetaData.WithoutPosition().Optimize());

    private static IEnumerable<MetaEntry<ConsoleFormat>> Optimize(this IEnumerable<MetaEntry<ConsoleFormat>> sourceEntries)
    {
        MetaEntry<ConsoleFormat>? last = null;

        foreach (var entry in sourceEntries)
        {
            if (entry.Text.Length == 0)
            {
                continue;
            }
            
            if (last is null)
            {
                last = entry;
                continue;
            }

            var lastEntry = last.Value;

            if (entry.DataEquals(lastEntry))
            {
                last = new(lastEntry.Text + entry.Text, lastEntry.Data);
            }
            else
            {
                yield return lastEntry;
                last = entry;
            }
        }
        if (last is {} l)
            yield return l;
    }
    
    internal static bool AreColorsEquivalent(Color? color1, Color? color2)
        => (color1 is null && color2 is null) || (color1 is not null && color2 is not null && color1.Value.ToArgb() == color2.Value.ToArgb()); 

}