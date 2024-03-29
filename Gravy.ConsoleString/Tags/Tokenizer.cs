using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Gravy.Ansi;

namespace Gravy.ConsoleString.Tags;

internal class Tokenizer
{
    private static readonly IDictionary<string, Ansi16Color> Ansi16ColorMap
        = Enum.GetNames<Ansi16Color>()
            .Zip(Enum.GetValues<Ansi16Color>())
            .ToDictionary(x => x.First, x => x.Second);
    
    private static readonly IDictionary<string, Ansi256Color> Ansi256ColorMap
        = Enum.GetNames<Ansi256Color>()
            .Zip(Enum.GetValues<Ansi256Color>())
            .ToDictionary(x => x.First, x => x.Second);

    // Allow inconsistent naming because it has no public properties or fields.
    // ReSharper disable InconsistentNaming
    private readonly string SourceText;
    private int Index;
    private readonly List<char> Buffer = new();
    private TokenizerState State = TokenizerState.Text;
    private TokenizerState? TargetState;
    private int CurrentLine = 1;
    private int CurrentColumn = 1;
    private int TokenStartLine = 1;
    private int TokenStartColumn = 1;
    // ReSharper restore InconsistentNaming
        
    private char Char => SourceText[Index];
    private string Text => new(Buffer.ToArray());

    private Tokenizer(string sourceText)
    {
        SourceText = sourceText;
    }

    public static IEnumerable<Token> Tokenize(string sourceText) => new Tokenizer(sourceText).GetTokens();

    private IEnumerable<Token> GetTokens()
    {
        while (Index < SourceText.Length)
        {
            switch (State)
            { 
                case TokenizerState.Text:
                    ParseTextChar();
                    MoveNext();
                    break;
                
                case TokenizerState.TextFinished:
                    Debug.Assert(TargetState is not null);
                    State = TargetState.Value;
                    TargetState = null;
                    if (Buffer.Count == 0)
                        break;
                    yield return Token.Text(Text, TokenStartLine, TokenStartColumn);
                    Buffer.Clear();
                    break;
                
                case TokenizerState.ParseStartTag:
                    ParseStartTag();
                    MoveNext();
                    break;
                
                case TokenizerState.ParseStopTag:
                    ParseStopTag();
                    MoveNext();
                    break;
                
                case TokenizerState.ResetAll:
                    yield return Token.ResetAll(TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StartForeground:
                    yield return Token.ForegroundStart(ReadCurrentColor(), TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StartBackground:
                    yield return Token.BackgroundStart(ReadCurrentColor(), TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StartBold:
                    yield return Token.WeightStart(FontWeight.Bold, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StartLight:
                    yield return Token.WeightStart(FontWeight.Light, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StartItalic:
                    yield return Token.StyleStart(FontStyle.Italic, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StartUnderline:
                    yield return Token.StyleStart(FontStyle.Underline, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StartBlink:
                    yield return Token.StyleStart(FontStyle.Blink, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StartInverse:
                    yield return Token.StyleStart(FontStyle.Inverse, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StartStrikeThrough:
                    yield return Token.StyleStart(FontStyle.StrikeThrough, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StopForeground:
                    yield return Token.ForegroundStop(TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StopBackground:
                    yield return Token.BackgroundStop(TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StopBold:
                    yield return Token.WeightStop(FontWeight.Bold, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StopLight:
                    yield return Token.WeightStop(FontWeight.Light, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StopItalic:
                    yield return Token.StyleStop(FontStyle.Italic, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StopUnderline:
                    yield return Token.StyleStop(FontStyle.Underline, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StopBlink:
                    yield return Token.StyleStop(FontStyle.Blink, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                case TokenizerState.StopInverse:
                    yield return Token.StyleStop(FontStyle.Inverse, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                    
                case TokenizerState.StopStrikeThrough:
                    yield return Token.StyleStop(FontStyle.StrikeThrough, TokenStartLine, TokenStartColumn);
                    CloseToken();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        if (State != TokenizerState.Text)
            throw new UnexpectedEndOfStringException(CurrentLine, CurrentColumn);
        if (Buffer.Count > 0)
            yield return Token.Text(Text, TokenStartLine, TokenStartColumn);
    }

    private void MoveNext()
    {
        Index++;
        CurrentColumn++;
        if (Index >= SourceText.Length || Char is not ('\r' or '\n'))
            return;
        
        Buffer.Add(Char);
        // Handle windows line endings (do not count \r\n as two lines)
        if (Char is '\r' && PeekNextChar() is '\n')
        {
            Buffer.Add(PeekNextChar()!.Value);
            Index++;
        }
        Index++;
        CurrentColumn = 1;
        CurrentLine++;
    }

    private void MoveTo(int index)
    {
        if (index == Index) return;
        if (index < Index) throw new InvalidOperationException("Cannot move backwards");
        while (index > Index)
            MoveNext();
    }

    private void ParseTextChar()
    {
        switch (Char)
        {
            case '\\':
                MoveNext();
                Buffer.Add(Char);
                break;
            case '[':
                State = TokenizerState.TextFinished;
                TokenStartColumn = CurrentColumn;
                TokenStartLine = CurrentLine;
                if (PeekNextChar() is '/')
                {
                    TargetState = TokenizerState.ParseStopTag;
                    MoveNext();
                }
                else
                {
                    TargetState = TokenizerState.ParseStartTag;
                }
                break;
            default:
                Buffer.Add(Char);
                break;

        }
    }

    private char? PeekNextChar()
    {
        if (Index + 1 >= SourceText.Length)
            return null;
        return SourceText[Index + 1];
    }

    private AnsiColor ReadCurrentColor()
    {
        var nextIndex = NextCloseBracketIndex();
        var data = SourceText[(Index + 1)..nextIndex];
        var color = Char switch
        {
            '#' => ReadColorHexValue(data),
            '!' => ReadColorNameValue(data),
            '@' => ReadColorThemeValue(data),
            '$' => ReadColorAnsi256Value(data),
            _ => throw new InvalidColorParserException(Char, CurrentLine, CurrentColumn),
        };
        MoveTo(nextIndex);
        return color;
    }

    private AnsiColor ReadColorThemeValue(string themeValue)
    {
        if (Ansi16ColorMap.TryGetValue(themeValue, out var color))
            return color;
        throw new UnknownThemeColorException("Unknown Color", themeValue, CurrentLine, CurrentColumn);
    }

    private AnsiColor ReadColorAnsi256Value(string ansi256Color)
    {
        if (Ansi256ColorMap.TryGetValue(ansi256Color, out var color))
            return color;
        throw new UnknownThemeColorException("Unknown Color", ansi256Color, CurrentLine, CurrentColumn);
    }
        
    private AnsiColor ReadColorHexValue(string hexValue)
    {
        try
        {
            return hexValue.Length switch
            {
                6 => Color.FromArgb(int.Parse("FF" + hexValue, NumberStyles.HexNumber)),
                8 => Color.FromArgb(int.Parse(hexValue, NumberStyles.HexNumber)),
                _ => throw new("Hex color value must be 6 or 8 characters long"),
            };
        }
        catch (Exception e)
        {
            throw new InvalidHexColorException(e.Message, hexValue, CurrentLine, CurrentColumn);
        }
    }

    private AnsiColor ReadColorNameValue(string colorName)
    {
        Color color;
        if ((color = Color.FromName(colorName)).ToKnownColor() != 0)
            return color;
        throw new UnknownNamedColorException(colorName, CurrentLine, CurrentColumn);
    }
        
    private void ParseStartTag()
    {
        State = Char switch
        {
            'F' => TokenizerState.StartForeground,
            'G' => TokenizerState.StartBackground,
            'B' => TokenizerState.StartBold,
            'L' => TokenizerState.StartLight,
            'U' => TokenizerState.StartUnderline,
            'I' => TokenizerState.StartItalic,
            'K' => TokenizerState.StartBlink,
            'V' => TokenizerState.StartInverse,
            'T' => TokenizerState.StartStrikeThrough,
            _ => throw new UnknownTagException(Char, TokenStartLine, TokenStartColumn),
        };
    }

    private void ParseStopTag()
    {
        State = Char switch
        {
            'F' => TokenizerState.StopForeground,
            'G' => TokenizerState.StopBackground,
            'B' => TokenizerState.StopBold,
            'L' => TokenizerState.StopLight,
            'U' => TokenizerState.StopUnderline,
            'I' => TokenizerState.StopItalic,
            'K' => TokenizerState.StopBlink,
            'V' => TokenizerState.StopInverse,
            'T' => TokenizerState.StopStrikeThrough,
            '/' => TokenizerState.ResetAll,
            _ => throw new UnknownTagException(Char, TokenStartLine, TokenStartColumn),
        };
    }

    private void CloseToken()
    {
        if (Char != ']')
            throw new MissingCloseBracketException(CurrentLine, CurrentColumn);
        MoveNext();
        State = TokenizerState.Text;
    }
    
    private int NextCloseBracketIndex()
    {
        int closeBracketIndex;
        try
        {
            closeBracketIndex = SourceText.IndexOf(']', Index);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new UnexpectedEndOfStringException(TokenStartLine, TokenStartColumn);
        }
        
        if (closeBracketIndex == -1)
            throw new UnexpectedEndOfStringException(TokenStartLine, TokenStartColumn);
        
        return closeBracketIndex;
    }

    private enum TokenizerState
    {
        Text,
        TextFinished,
        ParseStartTag,
        ParseStopTag,
        ResetAll,
        StartForeground,
        StartBackground,
        StartBold,
        StartLight,
        StartItalic,
        StartUnderline,
        StartBlink,
        StartInverse,
        StartStrikeThrough,
        StopForeground,
        StopBackground,
        StopBold,
        StopLight,
        StopItalic,
        StopUnderline,
        StopBlink,
        StopInverse,
        StopStrikeThrough,
    }
}