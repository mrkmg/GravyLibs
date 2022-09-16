using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gravy.Ansi;
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
    private FontWeight CurrentFontWeight => WeightStack.Count > 0 ? WeightStack.Peek().FontWeight : FontWeight.Normal;
    private AnsiColor? CurrentForeground => ForegroundStack.Count > 0 ? ForegroundStack.Peek().Color : null;
    private AnsiColor? CurrentBackground => BackgroundStack.Count > 0 ? BackgroundStack.Peek().Color : null;
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
                    yield return new(textToken.Text, CurrentFormat);
                    break;
                    
                case Token.BackgroundStartToken backgroundStartToken:
                    if (StrictMode && CurrentBackground is not null && CurrentBackground.Value == backgroundStartToken.Color)
                        throw new DuplicateColorException("Background", backgroundStartToken.Color, backgroundStartToken.Line, backgroundStartToken.Column);
                    BackgroundStack.Push(backgroundStartToken);
                    break;
                
                case Token.ForegroundStartToken foregroundStartToken:
                    if (StrictMode && CurrentForeground is not null && CurrentForeground.Value == foregroundStartToken.Color)
                        throw new DuplicateColorException("Foreground", foregroundStartToken.Color, foregroundStartToken.Line, foregroundStartToken.Column);
                    ForegroundStack.Push(foregroundStartToken);
                    break;
                
                case Token.WeightStartToken weightStartToken:
                    if (StrictMode && CurrentFontWeight == weightStartToken.FontWeight)
                        throw new DuplicateWeightException(weightStartToken.FontWeight, weightStartToken.Line, weightStartToken.Column);
                    WeightStack.Push(weightStartToken);
                    break;
                
                case Token.StyleStartToken styleStartToken:
                    if (StrictMode && FontStyleCounts[styleStartToken.FontStyle].Count > 0)
                        throw new DuplicateStyleException(styleStartToken.FontStyle, styleStartToken.Line, styleStartToken.Column);
                    FontStyleCounts[styleStartToken.FontStyle].Push(styleStartToken);
                    break;
                
                case Token.BackgroundStopToken:
                    if (StrictMode && BackgroundStack.Count == 0)
                        throw new UnmatchedStopTokenException("Background", token.Line, token.Column);
                    BackgroundStack.TryPop(out _);
                    break;
                
                case Token.ForegroundStopToken:
                    if (StrictMode && ForegroundStack.Count == 0)
                        throw new UnmatchedStopTokenException("Foreground", token.Line, token.Column);
                    ForegroundStack.TryPop(out _);
                    break;
                
                case Token.WeightStopToken weightStopToken:
                    if (StrictMode && WeightStack.Count == 0) 
                        throw new UnmatchedStopTokenException(weightStopToken.FontWeight.ToString(), weightStopToken.Line, weightStopToken.Column);
                    if (WeightStack.Count > 0 && CurrentFontWeight != weightStopToken.FontWeight)
                        throw new UnmatchedStopTokenException(weightStopToken.FontWeight.ToString(), weightStopToken.Line, weightStopToken.Column);
                    WeightStack.TryPop(out _);
                    break;
                
                case Token.StyleStopToken styleStopToken:
                    if (StrictMode && FontStyleCounts[styleStopToken.FontStyle].Count == 0)
                        throw new UnmatchedStopTokenException(styleStopToken.FontStyle.ToString(), styleStopToken.Line, styleStopToken.Column);
                    FontStyleCounts[styleStopToken.FontStyle].TryPop(out _);
                    break;
                
                case Token.ResetAllToken when StrictMode:
                    throw new ResetAllInStrictModeException(token.Line, token.Column);
                
                case Token.ResetAllToken:
                    BackgroundStack.Clear();
                    ForegroundStack.Clear();
                    WeightStack.Clear();
                    foreach (var style in Enum.GetValues<FontStyle>())
                        FontStyleCounts[style].Clear();
                    break;
                
                default:
                    Debug.Fail("Unexpected token type.");
                    break;
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