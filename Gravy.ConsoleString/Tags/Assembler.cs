using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Gravy.MetaString;

namespace Gravy.ConsoleString.Tags;

internal class Assembler
{
    // Allow inconsistent naming because it has no public properties or fields.
    // ReSharper disable InconsistentNaming
    private readonly Stack<Token.ForegroundStartToken> ForegroundStack = new();
    private readonly Stack<Token.BackgroundStartToken> BackgroundStack = new();
    private readonly Stack<Token.WeightStartToken> WeightStack = new();
    private readonly Dictionary<FontStyle, Stack<Token.StyleStartToken>> FontStyleCounts = new();
    private readonly bool StrictMode;
    // Resharper enable InconsistentNaming
    
    private FontStyle CurrentFontStyle 
        => FontStyleCounts
            .Where(sv => sv.Value.Count > 0)
            .Select(sv => sv.Key)
            .Aggregate(FontStyle.None, (current, style) => current | style);
    private FontWeight CurrentFontWeight => WeightStack.Count > 0 ? WeightStack.Peek().Value : FontWeight.Normal;
    private Color? CurrentForeground => ForegroundStack.Count > 0 ? ForegroundStack.Peek().Value : null;
    private Color? CurrentBackground => BackgroundStack.Count > 0 ? BackgroundStack.Peek().Value : null;
    private ConsoleFormat CurrentFormat => new(CurrentForeground, CurrentBackground, CurrentFontWeight, CurrentFontStyle);

    private Assembler(bool strictMode)
    {
        StrictMode = strictMode;
        foreach (var style in Enum.GetValues<FontStyle>()) 
            FontStyleCounts.Add(style, new());
    }
    
    private IEnumerable<MetaEntry<ConsoleFormat>> AssembleToEntries(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            switch (token)
            {
                case Token.TextToken textToken:
                    yield return new(textToken.Value, CurrentFormat);
                    break;
                case Token.BackgroundStartToken backgroundStartToken:
                    if (StrictMode && BackgroundStack.Count > 0 && BackgroundStack.Peek().Value == backgroundStartToken.Value)
                        throw new DuplicateColorException("Background", backgroundStartToken.Value, backgroundStartToken.Line, backgroundStartToken.Column);
                    BackgroundStack.Push(backgroundStartToken);
                    break;
                case Token.ForegroundStartToken foregroundStartToken:
                    if (StrictMode && ForegroundStack.Count > 0 && ForegroundStack.Peek().Value == foregroundStartToken.Value)
                        throw new DuplicateColorException("Foreground", foregroundStartToken.Value, foregroundStartToken.Line, foregroundStartToken.Column);
                    ForegroundStack.Push(foregroundStartToken);
                    break;
                case Token.WeightStartToken weightStartToken:
                    if (StrictMode && WeightStack.Count > 0 && WeightStack.Peek().Value == weightStartToken.Value)
                        throw new DuplicateWeightException(weightStartToken.Value, weightStartToken.Line, weightStartToken.Column);
                    WeightStack.Push(weightStartToken);
                    break;
                case Token.StyleStartToken styleStartToken:
                    if (StrictMode && FontStyleCounts[styleStartToken.Value].Count > 0)
                        throw new DuplicateStyleException(styleStartToken.Value, styleStartToken.Line, styleStartToken.Column);
                    FontStyleCounts[styleStartToken.Value].Push(styleStartToken);
                    break;
                case Token.BackgroundStopToken:
                    if (StrictMode && BackgroundStack.Count == 0)
                        throw new UnmatchedStopTokenException(token.Line, token.Column);
                    BackgroundStack.TryPop(out _);
                    break;
                case Token.ForegroundStopToken:
                    if (StrictMode && ForegroundStack.Count == 0)
                        throw new UnmatchedStopTokenException(token.Line, token.Column);
                    ForegroundStack.TryPop(out _);
                    break;
                case Token.WeightStopToken stopToken:
                    if (StrictMode && WeightStack.Count == 0) 
                        throw new UnmatchedStopTokenException(token.Line, token.Column);
                    if (WeightStack.Count > 0 && WeightStack.Peek().Value != stopToken.Value)
                        throw new UnmatchedStopTokenException(token.Line, token.Column);
                    WeightStack.TryPop(out _);
                    break;
                case Token.StyleStopToken styleStopToken:
                    if (StrictMode && FontStyleCounts[styleStopToken.Value].Count == 0)
                        throw new UnmatchedStopTokenException(token.Line, token.Column);
                    FontStyleCounts[styleStopToken.Value].TryPop(out _);
                    break;
                case Token.ResetAllToken:
                    // No reset in strict mode.
                    if (StrictMode)
                        throw new ResetAllInStrictModeException(token.Line, token.Column);
                    BackgroundStack.Clear();
                    ForegroundStack.Clear();
                    WeightStack.Clear();
                    foreach (var style in Enum.GetValues<FontStyle>())
                        FontStyleCounts[style].Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown Token type: {token.GetType().Name}");
            }
        }

        if (StrictMode)
            EnsureAllTokensClosed();
    }

    private void EnsureAllTokensClosed()
    {
        if (ForegroundStack.Count > 0)
        {
            var token = ForegroundStack.Peek();
            throw new UnmatchedStartTokenException(token.Line, token.Column);
        }

        if (BackgroundStack.Count > 0)
        {
            var token = BackgroundStack.Peek();
            throw new UnmatchedStartTokenException(token.Line, token.Column);
        }

        if (WeightStack.Count > 0)
        {
            var token = WeightStack.Peek();
            throw new UnmatchedStartTokenException(token.Line, token.Column);
        }

        foreach (var value in Enum.GetValues<FontStyle>())
        {
            if (FontStyleCounts[value].Count == 0) continue;
            
            var token = FontStyleCounts[value].Peek();
            throw new UnmatchedStartTokenException(token.Line, token.Column);
        }
    }

    public static MetaString<ConsoleFormat> Assemble(IEnumerable<Token> tokens, bool strict = false) 
        => new(new Assembler(strict).AssembleToEntries(tokens));
}