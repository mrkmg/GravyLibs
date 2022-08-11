using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;

namespace Gravy.ConsoleString.Tags;

internal class Tokenizer
{
    // Allow inconsistent naming because it has no public properties or fields.
    // ReSharper disable InconsistentNaming
    private readonly string SourceText;
    private int Index;
    private readonly List<char> Buffer = new();
    private TokenizerState State = TokenizerState.Text;
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
            Token token;
            switch (State)
            { 
                case TokenizerState.Text:
                    switch (Char)
                    {
                        case '\\':
                            Buffer.Add(TryPeekNextChar() ?? throw new UnexpectedEndOfStringException(CurrentLine, CurrentColumn));
                            Index++;
                            break;
                        case '[':
                            TokenStartColumn = CurrentColumn;
                            TokenStartLine = CurrentLine;
                            if (TryPeekNextChar() is '/')
                            {
                                State = TokenizerState.ParseStopTag;
                                Index++;
                            }
                            else
                            {
                                State = TokenizerState.ParseStartTag;
                            }
                            if (Buffer.Count > 0)
                                yield return Token.Text(Text, TokenStartLine, TokenStartColumn);
                            Buffer.Clear();
                            break;
                        case '\r' or '\n':
                            CurrentLine++;
                            CurrentColumn = 0;
                            Buffer.Add(Char);
                            if (Char is '\r' && TryPeekNextChar() is '\n')
                            {
                                Index++;
                                Buffer.Add(Char);
                            }
                            break;
                        default:
                            Buffer.Add(Char);
                            break;
                            
                    }
                    Index++;
                    break;
                case TokenizerState.ParseStartTag:
                    ParseStartTag();
                    Index++;
                    break;
                case TokenizerState.ParseStopTag:
                    ParseStopTag();
                    Index++;
                    break;
                case TokenizerState.ResetAll:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.ResetAll(TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StartForeground:
                    State = Char switch
                    {
                        '#' => TokenizerState.ParseForegroundHex,
                        '!' => TokenizerState.ParseForegroundName,
                        _ => throw new InvalidColorParserException(Char, CurrentLine, CurrentColumn)
                    };
                    Index++;
                    break;
                case TokenizerState.StartBackground:
                    State = Char switch
                    {
                        '#' => TokenizerState.ParseBackgroundHex,
                        '!' => TokenizerState.ParseBackgroundName,
                        _ => throw new InvalidColorParserException(Char, CurrentLine, CurrentColumn)
                    };
                    Index++;
                    break;
                case TokenizerState.ParseForegroundHex:
                    token = Token.ForegroundStart(ReadColorHexValue(SourceText[Index..NextCloseBracketIndex()]), TokenStartLine, TokenStartColumn);
                    Index = NextCloseBracketIndex() + 1;
                    State = TokenizerState.Text;
                    yield return token;
                    break;
                case TokenizerState.ParseBackgroundHex:
                    token = Token.BackgroundStart(ReadColorHexValue(SourceText[Index..NextCloseBracketIndex()]), TokenStartLine, TokenStartColumn);
                    Index = NextCloseBracketIndex() + 1;
                    State = TokenizerState.Text;
                    yield return token;
                    break;
                case TokenizerState.ParseForegroundName:
                    token = Token.ForegroundStart(ReadColorNameValue(SourceText[Index..NextCloseBracketIndex()]), TokenStartLine, TokenStartColumn);
                    Index = NextCloseBracketIndex() + 1;
                    State = TokenizerState.Text;
                    yield return token;
                    break;
                case TokenizerState.ParseBackgroundName:
                    token = Token.BackgroundStart(ReadColorNameValue(SourceText[Index..NextCloseBracketIndex()]), TokenStartLine, TokenStartColumn);
                    Index = NextCloseBracketIndex() + 1;
                    State = TokenizerState.Text;
                    yield return token;
                    break;
                case TokenizerState.StartBold:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.WeightStart(FontWeight.Bold, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StartLight:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.WeightStart(FontWeight.Light, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StartItalic:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStart(FontStyle.Italic, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StartUnderline:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStart(FontStyle.Underline, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StartBlink:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStart(FontStyle.Blink, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StartInverse:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStart(FontStyle.Inverse, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StartStrikeThrough:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStart(FontStyle.StrikeThrough, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StopForeground:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.ForegroundStop(TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StopBackground:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.BackgroundStop(TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StopBold:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.WeightStop(FontWeight.Bold, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StopLight:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.WeightStop(FontWeight.Light, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StopItalic:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStop(FontStyle.Italic, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StopUnderline:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStop(FontStyle.Underline, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StopBlink:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStop(FontStyle.Blink, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StopInverse:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStop(FontStyle.Inverse, TokenStartLine, TokenStartColumn);
                    break;
                case TokenizerState.StopStrikeThrough:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStop(FontStyle.StrikeThrough, TokenStartLine, TokenStartColumn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CurrentColumn++;
        }
        if (State != TokenizerState.Text)
            throw new UnexpectedEndOfStringException(CurrentLine, CurrentColumn);
        if (Buffer.Count > 0)
            yield return Token.Text(Text, TokenStartLine, TokenStartColumn);
    }
    
    private char? TryPeekNextChar()
    {
        if (Index + 1 >= SourceText.Length)
            return null;
        return SourceText[Index + 1];
    }
        
    private Color ReadColorHexValue(string hexValue)
    {
        try
        {
            return hexValue.Length switch
            {
                6 => Color.FromArgb(int.Parse("FF" + hexValue, NumberStyles.HexNumber)),
                8 => Color.FromArgb(int.Parse(hexValue, NumberStyles.HexNumber)),
                _ => throw new InvalidHexColorException(hexValue, CurrentLine, CurrentColumn)
            };
        }
        catch (FormatException)
        {
            throw new InvalidHexColorException(hexValue, CurrentLine, CurrentColumn);
        }
    }

    private Color ReadColorNameValue(string colorName)
    {
        var color = Color.FromName(colorName);
        if (color.ToKnownColor() == 0) 
            throw new UnknownNamedColorException(colorName, CurrentLine, CurrentColumn);
        return color;
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
            _ => throw new UnknownTagException(Char, TokenStartLine, TokenStartColumn)
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
            _ => throw new UnknownTagException(Char, TokenStartLine, TokenStartColumn)
        };
    }

    private void VerifyCloseBracketAndProgressIndex()
    {
        if (Char != ']')
            throw new MissingCloseBracketException(CurrentLine, CurrentColumn);
        Index++;
        State = TokenizerState.Text;
    }

    private int NextCloseBracketIndex()
    {
        int closeBracketIndex;
        try
        {
            closeBracketIndex = SourceText.IndexOf(']', Index);
        } catch (ArgumentOutOfRangeException) { throw new UnexpectedEndOfStringException(TokenStartLine, TokenStartColumn); }
        if (closeBracketIndex == -1)
            throw new UnexpectedEndOfStringException(TokenStartLine, TokenStartColumn);
        return closeBracketIndex;
    }

    private enum TokenizerState
    {
        Text,
        ParseStartTag,
        ParseStopTag,
        ResetAll,
        StartForeground,
        StartBackground,
        ParseForegroundHex,
        ParseBackgroundHex,
        ParseForegroundName,
        ParseBackgroundName,
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