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

    public ConsoleString(string text, Color? foregroundColor = null, Color? backgroundColor = null, FontStyle style = FontStyle.None)
        : this(text, new(foregroundColor, backgroundColor, style)) { }

    public ConsoleString(string str) : base(str, ConsoleFormat.Default) { }
    public ConsoleString(string text, ConsoleFormat format) : base(text, format) { }
    public ConsoleString(IEnumerable<MetaEntry<ConsoleFormat>> entries) : base(entries) { }

    internal ConsoleString(PositionedMetaEntry<ConsoleFormat>[] metaEntries) : base(metaEntries) { }
    internal ConsoleString(IEnumerable<ConsoleString> consoleStrings) : base(consoleStrings.SelectMany(cs => cs.MetaEntries).ToArray()) { }

    public ConsoleString With(ConsoleFormat format)
        => new(RawText, format);

    public ConsoleString With(Color? foreground, Color? background, FontStyle style)
        => With(new(foreground, background, style));

    public ConsoleString WithForeground(Color? color)
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithForeground(color))));
    public ConsoleString WithBackground(Color? color)
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithBackground(color))));
    
    public ConsoleString WithStyle(FontStyle style)
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithStyle(style))));
    public ConsoleString WithoutStyle(FontStyle style)
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.WithoutStyle(style))));
    
    public ConsoleString ResetStyle()
        => new(MetaEntries.Select(x => new MetaEntry<ConsoleFormat>(x.Text, x.Data.ResetStyle())));
    
    public ConsoleString WithBold() => WithStyle(FontStyle.Bold);
    public ConsoleString WithItalic() => WithStyle(FontStyle.Italic);
    public ConsoleString WithUnderline() => WithStyle(FontStyle.Underline);
    public ConsoleString WithoutBold() => WithoutStyle(FontStyle.Bold);
    public ConsoleString WithoutItalic() => WithoutStyle(FontStyle.Italic);
    public ConsoleString WithoutUnderline() => WithoutStyle(FontStyle.Underline);
    
    public override string ToString() => ToEscapedString();
    
    public string ToTaggedString(bool useReset = false)
    {
        if (MetaEntries.Length == 0)
            return string.Empty;

        var currentFormat = ConsoleFormat.Default;
        var sb = new StringBuilder();

        void WriteFormatDiff(ConsoleFormat format)
        {
            if (useReset && format.Equals(ConsoleFormat.Default) && !currentFormat.Equals(ConsoleFormat.Default))
            {
                sb.Append("[//]");
                return;
            }
            
            if (format.BackgroundColor != currentFormat.BackgroundColor)
            {
                if (format.BackgroundColor.HasValue)
                    sb.Append($@"[G{format.BackgroundColor.Value.ToCsColor()}]");
                else
                    sb.Append(@"[/G]");
            }
            if (format.ForegroundColor != currentFormat.ForegroundColor)
            {
                if (format.ForegroundColor.HasValue)
                    sb.Append($@"[F{format.ForegroundColor.Value.ToCsColor()}]");
                else
                    sb.Append(@"[/F]");
            }
            
            if (format.Styles.Equals(currentFormat.Styles)) return;
            
            if (!currentFormat.Styles.HasFlag(FontStyle.Bold) && format.Styles.HasFlag(FontStyle.Bold))
                sb.Append(@"[B]");
            if (currentFormat.Styles.HasFlag(FontStyle.Bold) && !format.Styles.HasFlag(FontStyle.Bold))
                sb.Append(@"[/B]");
                
            if (!currentFormat.Styles.HasFlag(FontStyle.Italic) && format.Styles.HasFlag(FontStyle.Italic))
                sb.Append("[I]");
            if (currentFormat.Styles.HasFlag(FontStyle.Italic) && !format.Styles.HasFlag(FontStyle.Italic))
                sb.Append("[/I]");
                
            if (!currentFormat.Styles.HasFlag(FontStyle.Underline) && format.Styles.HasFlag(FontStyle.Underline))
                sb.Append("[U]");
            if (currentFormat.Styles.HasFlag(FontStyle.Underline) && !format.Styles.HasFlag(FontStyle.Underline))
                sb.Append("[/U]");
        } 
        
        foreach (var entry in Optimize().MetaEntries)
        {
            WriteFormatDiff(entry.Data);
            sb.Append(entry.Text);
            currentFormat = entry.Data;
        }
        WriteFormatDiff(ConsoleFormat.Default);
        return sb.ToString();
    }
    
    public string ToEscapedString()
    {
        if (MetaEntries.Length == 0)
            return string.Empty;
        
        var sb = new StringBuilder();
        var currentFormat = new ConsoleFormat();
        foreach (var entry in Optimize().MetaEntries)
        {
            if (entry.Data.Equals(ConsoleFormat.Default) && !currentFormat.Equals(ConsoleFormat.Default))
            {
                sb.SetMode(Mode.Reset);
                currentFormat = entry.Data;
            }
            
            if (!ColorsEquivalent(entry.Data.BackgroundColor, currentFormat.BackgroundColor))
            {
                if (entry.Data.BackgroundColor.HasValue)
                    sb.SetBackgroundColor(entry.Data.BackgroundColor.Value.ToAnsi());
                else
                    sb.SetMode(Mode.BackgroundDefault);
            }
            if (!ColorsEquivalent(entry.Data.ForegroundColor, currentFormat.ForegroundColor))
            {
                if (entry.Data.ForegroundColor.HasValue)
                    sb.SetForegroundColor(entry.Data.ForegroundColor.Value.ToAnsi());
                else
                    sb.SetMode(Mode.ForegroundDefault);
            }
            if (entry.Data.Styles != currentFormat.Styles)
            {
                if (currentFormat.Styles != FontStyle.None)
                    sb.SetMode(Mode.Normal);
                if (entry.Data.Styles.HasFlag(FontStyle.Bold))
                    sb.SetMode(Mode.Bold);
                if (entry.Data.Styles.HasFlag(FontStyle.Italic))
                    sb.SetMode(Mode.Italic);
                if (entry.Data.Styles.HasFlag(FontStyle.Underline))
                    sb.SetMode(Mode.Underline);
            }
            sb.Append(entry.Text);
            currentFormat = entry.Data;
        }
        if (!currentFormat.Equals(ConsoleFormat.Default))
            sb.SetMode(Mode.Reset);
        return sb.ToString();
    }

    public ConsoleString Optimize()
        => new(Optimize(MetaEntries));

    public static implicit operator ConsoleString(string str)
        => new(str, default!);

    [DebuggerHidden]
    private string DebugView => ToTaggedString(true);
    
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
                var str = string.Join(null, sourceEntries[sourceIdx..n].Select(e => e.Text));
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

    private static bool ColorsEquivalent(Color? color1, Color? color2)
        => color1 is null && color2 is null || (color1 is not null && color2 is not null && color1.Value.ToArgb() == color2.Value.ToArgb()); 

}