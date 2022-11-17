using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Gravy.Ansi;
using Gravy.ConsoleString.Tags;
using Gravy.MetaString;


namespace Gravy.ConsoleString;

using ConsoleString = MetaString<ConsoleFormat>;

public static class MetaStringConsoleFormat
{
    private static ConsoleString Transform(this ConsoleString cs, Func<ConsoleFormat, ConsoleFormat> action) 
        => new(cs.MetaData.Select(x => new MetaEntry<ConsoleFormat>(x.Text, action(x.Data))));

    public static ConsoleString From(this string str, ConsoleFormat format) => new (str, format);
    public static ConsoleString With(this ConsoleString cs, ConsoleFormat format) => new (cs.RawText, format);
    public static ConsoleString WithForeground(this ConsoleString cs, AnsiColor? color) => cs.Transform(x => x.WithForeground(color));
    public static ConsoleString WithBackground(this ConsoleString cs, AnsiColor? color) => cs.Transform(x => x.WithBackground(color));
    public static ConsoleString WithStyle(this ConsoleString cs, FontStyle style) => cs.Transform(x => x.WithStyle(style));
    public static ConsoleString WithoutStyle(this ConsoleString cs, FontStyle style) => cs.Transform(x => x.WithoutStyle(style));
    public static ConsoleString WithWeight(this ConsoleString cs, FontWeight weight) => cs.Transform(x => x.WithWeight(weight));
    public static ConsoleString ResetStyle(this ConsoleString cs) => cs.Transform(x => x.ResetStyle());
    public static ConsoleString WithNormal(this ConsoleString cs) => cs.Transform(x => x.WithNormal());
    public static ConsoleString WithBold(this ConsoleString cs) => cs.Transform(x => x.WithBold());
    public static ConsoleString WithLight(this ConsoleString cs) => cs.Transform(x => x.WithLight());
    public static ConsoleString WithItalic(this ConsoleString cs) => cs.Transform(x => x.WithItalic());
    public static ConsoleString WithoutItalic(this ConsoleString cs) => cs.Transform(x => x.WithoutItalic());
    public static ConsoleString WithUnderline(this ConsoleString cs) => cs.Transform(x => x.WithUnderline());
    public static ConsoleString WithoutUnderline(this ConsoleString cs) => cs.Transform(x => x.WithoutUnderline());
    public static ConsoleString WithBlink(this ConsoleString cs) => cs.Transform(x => x.WithBlink());
    public static ConsoleString WithoutBlink(this ConsoleString cs) => cs.Transform(x => x.WithoutBlink());
    public static ConsoleString WithInverse(this ConsoleString cs) => cs.Transform(x => x.WithInverse());
    public static ConsoleString WithoutInverse(this ConsoleString cs) => cs.Transform(x => x.WithoutInverse());
    public static ConsoleString WithStrikeThrough(this ConsoleString cs) => cs.Transform(x => x.WithStrikeThrough());
    public static ConsoleString WithoutStrikeThrough(this ConsoleString cs) => cs.Transform(x => x.WithoutStrikeThrough());
    public static ConsoleString AsConsoleString(this string str) => new(str);
    public static ConsoleString With(this string str, ConsoleFormat format) => new(str, format);
    public static ConsoleString With(this string str, FontWeight weight) => new(str, new(null, null, weight));
    public static ConsoleString With(this string str, FontStyle style) => new(str, new(null, null, default, style));
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
            _ => throw new ("Missing color type"),
        };
    }
    
    /// <summary>
    /// Parses a string with ConsoleStringTags into a ConsoleString
    /// </summary>
    /// <param name="str">The input formatted string.</param>
    /// <param name="strictMode">Enable on strict mode</param>
    /// <returns>The parsed ConsoleString</returns>
    /// <exception cref="System.Exception">Parser Errors</exception>
    /// <remarks>
    /// <para>Tags:</para>
    /// <b>[F#XXXXXX]</b> Set foreground color to a RGB color.<br />
    /// <b>[F!Name]</b>   Set foreground color to a known color name. See <see cref="KnownColor"/><br />
    /// <b>[F@Name]</b>   Set foreground color to an ANSI16 color name. See <see cref="Ansi16Color"/><br />
    /// <b>[F$Name]</b>   Set foreground color to an ANSI256 color name. See <see cref="Ansi256Color"/><br />
    /// <b>[/F]</b>       Stop the current foreground color.<br />
    /// <b>[G#XXXXXX]</b> Set background color to a RGB color.<br />
    /// <b>[G!Name]</b>   Set background color to a known color name. See <see cref="KnownColor"/><br />
    /// <b>[G@Name]</b>   Set background color to an ANSI color name. See <see cref="Ansi16Color"/><br />
    /// <b>[G$Name]</b>   Set background color to an ANSI256 color name. See <see cref="Ansi256Color"/><br />
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
    /// <b>[K]</b>        Enable Blink.<br />
    /// <b>[/K]</b>       Disable Blink.<br />
    /// <b>[V]</b>        Enable Inverse.<br />
    /// <b>[/V]</b>       Disable Inverse.<br />
    /// <b>[//]</b>       Reset all styles.
    /// </remarks>
    public static ConsoleString ParseTags(this string str, bool strictMode = false) 
        => Assembler.Assemble(Tokenizer.Tokenize(str), strictMode).Optimize();
    
    public static string EscapeTags(this string str) => string.Join(@"\[", str.Split('['));

    public static void WriteAnsiToConsole(this ConsoleString cs) => cs.WriteAnsiTo(Console.Out);
    
    public static void WriteAnsiTo(this ConsoleString cs, TextWriter writer)
    {
        if (!cs.MetaData.Any())
            return;

        var currentFormat = ConsoleFormat.Default;
        foreach (var entry in cs.MetaData)
        {
            WriteFormatDiff(writer, currentFormat, entry.Data);
            writer.Write(entry.Text);
            currentFormat = entry.Data;
        }
        WriteFormatDiff(writer, currentFormat, ConsoleFormat.Default);
    }
    
    public static string ToAnsiString(this ConsoleString cs)
    {
        var writer = new StringWriter();
        cs.WriteAnsiTo(writer);
        return writer.ToString();
    }
    
    private static void WriteFormatDiff(TextWriter textWriter, ConsoleFormat fromFormat, ConsoleFormat toFormat)
    {
        if (toFormat.Equals(ConsoleFormat.Default) && !fromFormat.Equals(ConsoleFormat.Default))
        {
            textWriter.SetMode(AnsiMode.Reset);
            return;
        }
        
        var modes = new List<AnsiMode>();
        
        if (toFormat.BackgroundColor != fromFormat.BackgroundColor)
            if (toFormat.BackgroundColor.HasValue)
                textWriter.SetBackgroundColor(toFormat.BackgroundColor.Value);
            else
                textWriter.SetMode(AnsiMode.BackgroundDefault);

        if (toFormat.ForegroundColor != fromFormat.ForegroundColor)
            if (toFormat.ForegroundColor.HasValue)
                textWriter.SetForegroundColor(toFormat.ForegroundColor.Value);
            else
                textWriter.SetMode(AnsiMode.ForegroundDefault);

        if (toFormat.Weight != fromFormat.Weight)
            modes.Add(toFormat.Weight switch
            {
                FontWeight.Bold => AnsiMode.Bold,
                FontWeight.Light => AnsiMode.Faint,
                FontWeight.Normal => AnsiMode.Normal,
                _ => throw new ("Missing definition for FontWeight"),
            });

        if (!fromFormat.Styles.HasFlag(FontStyle.Italic) && toFormat.Styles.HasFlag(FontStyle.Italic))
            modes.Add(AnsiMode.Italic);
        if (fromFormat.Styles.HasFlag(FontStyle.Italic) && !toFormat.Styles.HasFlag(FontStyle.Italic))
            modes.Add(AnsiMode.NoItalic);
            
        if (!fromFormat.Styles.HasFlag(FontStyle.Underline) && toFormat.Styles.HasFlag(FontStyle.Underline))
            modes.Add(AnsiMode.Underline);
        if (fromFormat.Styles.HasFlag(FontStyle.Underline) && !toFormat.Styles.HasFlag(FontStyle.Underline))
            modes.Add(AnsiMode.NoUnderline);
            
        if (!fromFormat.Styles.HasFlag(FontStyle.Blink) && toFormat.Styles.HasFlag(FontStyle.Blink))
            modes.Add(AnsiMode.Blink);
        if (fromFormat.Styles.HasFlag(FontStyle.Blink) && !toFormat.Styles.HasFlag(FontStyle.Blink))
            modes.Add(AnsiMode.NoBlink);
            
        if (!fromFormat.Styles.HasFlag(FontStyle.Inverse) && toFormat.Styles.HasFlag(FontStyle.Inverse))
            modes.Add(AnsiMode.Inverse);
        if (fromFormat.Styles.HasFlag(FontStyle.Inverse) && !toFormat.Styles.HasFlag(FontStyle.Inverse))
            modes.Add(AnsiMode.NoInverse);
            
        if (!fromFormat.Styles.HasFlag(FontStyle.StrikeThrough) && toFormat.Styles.HasFlag(FontStyle.StrikeThrough))
            modes.Add(AnsiMode.StrikeThrough);
        if (fromFormat.Styles.HasFlag(FontStyle.StrikeThrough) && !toFormat.Styles.HasFlag(FontStyle.StrikeThrough))
            modes.Add(AnsiMode.NoStrikeThrough);

        if (modes.Count > 0)
            textWriter.SetMode(modes.ToArray());
    } 
    
    public static string ToTaggedString(this ConsoleString cs, bool useReset = false)
    {
        if (!cs.MetaData.Any())
            return string.Empty;

        var currentFormat = ConsoleFormat.Default;
        var sb = new StringBuilder();
        foreach (var entry in cs.MetaData)
        {
            AppendTaggedStringFormatDiff(sb, currentFormat, entry.Data, useReset);
            sb.Append(entry.Text);
            currentFormat = entry.Data;
        }
        AppendTaggedStringFormatDiff(sb, currentFormat, ConsoleFormat.Default, useReset);
        return sb.ToString();
    }
    
    private static void AppendTaggedStringFormatDiff(StringBuilder sb, ConsoleFormat fromFormat, ConsoleFormat toFormat, bool useReset)
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

}