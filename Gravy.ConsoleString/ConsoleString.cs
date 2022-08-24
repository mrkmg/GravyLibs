using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Ansi;
using Gravy.MetaString;

namespace Gravy.ConsoleString;



[DebuggerDisplay("{DebugView}")]
[MetaStringGenerator(nameof(ConsoleFormat))]
public partial class ConsoleString : ICloneable
{
    public static void EnableWindowsSupport()
        => WindowsConsole.TryEnableVirtualTerminalProcessing();

    public ConsoleString(ConsoleStringInterpolatedStringHandler handler) : base(handler.ToConsoleString().MetaEntries) { }

    public ConsoleString(string text, Color? foregroundColor = null, Color? backgroundColor = null, FontWeight weight = FontWeight.Normal, FontStyle style = FontStyle.None)
        : this(text, new(foregroundColor, backgroundColor, weight, style)) { }

    public ConsoleString(string str) : base(str, ConsoleFormat.Default) { }
    public ConsoleString(string text, ConsoleFormat format) : base(text, format) { }
    public ConsoleString(IEnumerable<MetaEntry<ConsoleFormat>> entries) : base(entries) { }
    
    internal ConsoleString(IEnumerable<PositionedMetaEntry<ConsoleFormat>> entries) : this(entries.ToArray()) { }
    internal ConsoleString(PositionedMetaEntry<ConsoleFormat>[] metaEntries) : base(Optimize(metaEntries)) { }
    internal ConsoleString(IEnumerable<ConsoleString> consoleStrings) : base(consoleStrings.SelectMany(cs => cs.MetaEntries).ToArray()) { }

    public ConsoleString With(ConsoleFormat format)
        => new(RawText, format);

    public ConsoleString With(Color? foreground, Color? background, FontWeight weight, FontStyle style)
        => With(new(foreground, background, weight, style));

    public ConsoleString WithForeground(Color? color)
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithForeground(color))));
    public ConsoleString WithBackground(Color? color)
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithBackground(color))));
    
    public ConsoleString WithStyle(FontStyle style)
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithStyle(style))));
    public ConsoleString WithoutStyle(FontStyle style)
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithoutStyle(style))));
    
    public ConsoleString WithWeight(FontWeight weight)
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithWeight(weight))));
    
    public ConsoleString ResetStyle()
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.ResetStyle())));
    
    public ConsoleString WithNormal() => WithWeight(FontWeight.Normal);
    public ConsoleString WithBold() => WithWeight(FontWeight.Bold);
    public ConsoleString WithLight() => WithWeight(FontWeight.Light);
    
    public ConsoleString WithItalic() => WithStyle(FontStyle.Italic);
    public ConsoleString WithoutItalic() => WithoutStyle(FontStyle.Italic);
    
    public ConsoleString WithUnderline() => WithStyle(FontStyle.Underline);
    public ConsoleString WithoutUnderline() => WithoutStyle(FontStyle.Underline);
    
    public ConsoleString WithBlink() => WithStyle(FontStyle.Blink);
    public ConsoleString WithoutBlink() => WithoutStyle(FontStyle.Blink);
    
    public ConsoleString WithInverse() => WithStyle(FontStyle.Inverse);
    public ConsoleString WithoutInverse() => WithoutStyle(FontStyle.Inverse);
    
    public ConsoleString WithStrikeThrough() => WithStyle(FontStyle.StrikeThrough);
    public ConsoleString WithoutStrikeThrough() => WithoutStyle(FontStyle.StrikeThrough);
    
    public override string ToString() => ToAnsiString();
    
    public string ToTaggedString(bool useReset = false)
    {
        if (MetaEntries.Length == 0)
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
        foreach (var entry in MetaEntries)
        {
            WriteFormatDiff(sb, currentFormat, entry.Data);
            sb.Append(entry.Text);
            currentFormat = entry.Data;
        }
        WriteFormatDiff(sb, currentFormat, ConsoleFormat.Default);
        return sb.ToString();
    }
    
    public string ToAnsiString()
    {
        if (MetaEntries.Length == 0)
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
        foreach (var entry in MetaEntries)
        {
            WriteFormatDiff(sb, currentFormat, entry.Data);
            sb.Append(entry.Text);
            currentFormat = entry.Data;
        }
        WriteFormatDiff(sb, currentFormat, ConsoleFormat.Default);
        return sb.ToString();
    }

    public ConsoleString Optimize()
        => new(Optimize(MetaEntries));

    public static implicit operator ConsoleString(string str)
        => new(str, default!);

    [DebuggerHidden]
    private string DebugView => ToTaggedString();
    
    private static PositionedMetaEntry<ConsoleFormat>[] Optimize(PositionedMetaEntry<ConsoleFormat>[] sourceEntries)
    {
        var newEntries = new PositionedMetaEntry<ConsoleFormat>[sourceEntries.Length];
        var sourceIdx = 0;
        var targetIdx = 0;
        var next = 0;
        
        void AddEntry(PositionedMetaEntry<ConsoleFormat> entry)
        {
            newEntries[targetIdx] = entry.PositionedAt(next);
            next += entry.Text.Length;
            targetIdx++;
        }
        
        while (sourceIdx < sourceEntries.Length)
        {
            if (sourceEntries[sourceIdx].Text.Length == 0)
            {
                sourceIdx++;
                continue;
            }
            
            var n = sourceIdx + 1;
            if (n >= sourceEntries.Length)
            {
                AddEntry(sourceEntries[sourceIdx]);
                break;
            }
            while (n < sourceEntries.Length && sourceEntries[sourceIdx].DataEquals(sourceEntries[n]))
                n++;
            if (n - 1 > sourceIdx)
            {
                var str = string.Join(null, sourceEntries.Skip(sourceIdx).Take(n - sourceIdx).Select(e => e.Text));
                AddEntry(new (next, str, sourceEntries[sourceIdx].Data));
                sourceIdx = n;
            }
            else
            {
                AddEntry(sourceEntries[sourceIdx]);
                sourceIdx++;
            }

        }
        return newEntries[..targetIdx];
    }

    private static bool AreColorsEquivalent(Color? color1, Color? color2)
        => (color1 is null && color2 is null) || (color1 is not null && color2 is not null && color1.Value.ToArgb() == color2.Value.ToArgb()); 

}