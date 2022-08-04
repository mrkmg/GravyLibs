using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Gravy.MetaString;

namespace Gravy.ConsoleString.Parser;

internal class ConsoleStringTagsTokenizer
{
    // Allow inconsistent naming because it has no public properties or fields.
    // ReSharper disable InconsistentNaming
    private readonly string SourceText;
    private int Index;
    private readonly List<char> Buffer = new();
    private ParserState State = ParserState.Text;
    // ReSharper restore InconsistentNaming
        
    private char Char => SourceText[Index];
    private string Text => new(Buffer.ToArray());

    private ConsoleStringTagsTokenizer(string sourceText)
    {
        SourceText = sourceText;
    }

    public static IEnumerable<Token> Tokenize(string sourceText) => new ConsoleStringTagsTokenizer(sourceText).GetTokens();

    private IEnumerable<Token> GetTokens()
    {
        while (Index < SourceText.Length)
        {
            Token token;
            switch (State)
            {
                case ParserState.Text:
                    switch (Char)
                    {
                        case '\\':
                            Index++;
                            break;
                        case '[':
                            Index++;
                            if (Char == '/')
                            {
                                State = ParserState.ParseStopTag;
                                Index++;
                            }
                            else
                            {
                                State = ParserState.ParseStartTag;
                            }
                            if (Buffer.Count > 0)
                                yield return Token.Text(Text);
                            break;
                    }
                    Buffer.Add(Char);
                    Index++;
                    break;
                case ParserState.ParseStartTag:
                    ParseStartTag();
                    Index++;
                    break;
                case ParserState.ParseStopTag:
                    ParseStopTag();
                    Index++;
                    break;
                case ParserState.ResetAll:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.ResetAll();
                    break;
                case ParserState.StartForeground:
                    State = Char switch
                    {
                        '#' => ParserState.ParseForegroundHex,
                        '!' => ParserState.ParseForegroundName,
                        _ => throw new InvalidColorParserException(Char, Index)
                    };
                    Index++;
                    break;
                case ParserState.StartBackground:
                    State = Char switch
                    {
                        '#' => ParserState.ParseBackgroundHex,
                        '!' => ParserState.ParseBackgroundName,
                        _ => throw new InvalidColorParserException(Char, Index)
                    };
                    Index++;
                    break;
                case ParserState.ParseForegroundHex:
                    token = Token.ForegroundStart(ReadColorHexValue(SourceText[Index..NextCloseBracketIndex()]));
                    Index = NextCloseBracketIndex() + 1;
                    State = ParserState.Text;
                    yield return token;
                    break;
                case ParserState.ParseBackgroundHex:
                    token = Token.BackgroundStart(ReadColorHexValue(SourceText[Index..NextCloseBracketIndex()]));
                    Index = NextCloseBracketIndex() + 1;
                    State = ParserState.Text;
                    yield return token;
                    break;
                case ParserState.ParseForegroundName:
                    token = Token.ForegroundStart(ReadColorNameValue(SourceText[Index..NextCloseBracketIndex()]));
                    Index = NextCloseBracketIndex() + 1;
                    State = ParserState.Text;
                    yield return token;
                    break;
                case ParserState.ParseBackgroundName:
                    token = Token.BackgroundStart(ReadColorNameValue(SourceText[Index..NextCloseBracketIndex()]));
                    Index = NextCloseBracketIndex() + 1;
                    State = ParserState.Text;
                    yield return token;
                    break;
                case ParserState.StartBold:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStart(FontStyle.Bold);
                    break;
                case ParserState.StartItalic:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStart(FontStyle.Italic);
                    break;
                case ParserState.StartUnderline:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStart(FontStyle.Underline);
                    break;
                case ParserState.StopForeground:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.ForegroundStop();
                    break;
                case ParserState.StopBackground:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.BackgroundStop();
                    break;
                case ParserState.StopBold:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStop(FontStyle.Bold);
                    break;
                case ParserState.StopItalic:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStop(FontStyle.Italic);
                    break;
                case ParserState.StopUnderline:
                    VerifyCloseBracketAndProgressIndex();
                    yield return Token.StyleStop(FontStyle.Underline);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        if (State != ParserState.Text)
            throw new UnexpectedEndOfStringException(Index - 1);
        if (Buffer.Count > 0)
            yield return Token.Text(Text);
    }
        
    private Color ReadColorHexValue(string hexValue)
        => hexValue.Length switch
        {
            6 => Color.FromArgb(int.Parse("FF" + hexValue, NumberStyles.HexNumber)),
            8 => Color.FromArgb(int.Parse(hexValue, NumberStyles.HexNumber)),
            _ => throw new InvalidHexColorException(hexValue, Index)
        };

    private Color ReadColorNameValue(string colorName)
    {
        var color = Color.FromName(colorName);
        if (color.Equals(Color.FromKnownColor(0))) throw new UnknownNamedColorException(colorName, Index);
        return color;
    }
        
    private void ParseStartTag()
    {
        State = Char switch
        {
            'F' => ParserState.StartForeground,
            'G' => ParserState.StartBackground,
            'B' => ParserState.StartBold,
            'U' => ParserState.StartUnderline,
            'I' => ParserState.StartItalic,
            '/' => ParserState.ParseStopTag,
            _ => throw new UnknownTagException(Char, Index)
        };
    }

    private void ParseStopTag()
    {
        State = Char switch
        {
            'F' => ParserState.StopForeground,
            'G' => ParserState.StopBackground,
            'B' => ParserState.StopBold,
            'U' => ParserState.StopUnderline,
            'I' => ParserState.StopItalic,
            '/' => ParserState.ResetAll,
            _ => throw new UnknownTagException(Char, Index)
        };
    }

    private void VerifyCloseBracketAndProgressIndex()
    {
        if (Char != ']')
            throw new MissingCloseBracketException(Index);
        Index++;
        State = ParserState.Text;
    }

    private int NextCloseBracketIndex()
    {
        int closeBracketIndex;
        try
        {
            closeBracketIndex = SourceText.IndexOf(']', Index);
        } catch (ArgumentOutOfRangeException) { throw new UnexpectedEndOfStringException(Index); }
        return closeBracketIndex;
    }
}

internal class ConsoleStringAssembler
{
    
    // Allow inconsistent naming because it has no public properties or fields.
    // ReSharper disable InconsistentNaming
    private ConsoleFormat CurrentFormat = ConsoleFormat.Default;
    // Resharper enable InconsistentNaming
    
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
                    CurrentFormat = CurrentFormat.WithBackground(backgroundStartToken.Value);
                    break;
                case Token.ForegroundStartToken foregroundStartToken:
                    CurrentFormat = CurrentFormat.WithForeground(foregroundStartToken.Value);
                    break;
                case Token.StyleStartToken styleStartToken:
                    CurrentFormat = CurrentFormat.WithStyle(styleStartToken.Value);
                    break;
                case Token.BackgroundStopToken:
                    CurrentFormat = CurrentFormat.WithBackground(null);
                    break;
                case Token.ForegroundStopToken:
                    CurrentFormat = CurrentFormat.WithForeground(null);
                    break;
                case Token.StyleStopToken styleStopToken:
                    CurrentFormat = CurrentFormat.WithStyle(styleStopToken.Value);
                    break;
                case Token.ResetAllToken:
                    CurrentFormat = ConsoleFormat.Default;
                    break;
            }
        }
    }
    
    public static ConsoleString Assemble(IEnumerable<Token> tokens) 
        => new(new ConsoleStringAssembler().AssembleToEntries(tokens));
}


internal class Token
{
    private readonly object? _value;

    private Token(object? value)
    {
        _value = value;
    }
    
    public static TextToken Text(string value) => new(value);
    public static ForegroundStartToken ForegroundStart(Color value) => new(value);
    public static BackgroundStartToken BackgroundStart(Color value) => new(value);
    public static ForegroundStopToken ForegroundStop() => new();
    public static BackgroundStopToken BackgroundStop() => new();
    public static StyleStartToken StyleStart(FontStyle value) => new(value);
    public static StyleStopToken StyleStop(FontStyle value) => new(value);
    public static ResetAllToken ResetAll() => new();
    
    public class TextToken : Token
    {
        public string Value => (string)(_value ?? throw new InvalidOperationException($"TextToken should have a value"));
        public TextToken(string value) : base(value) { }
    }
    public class ForegroundStartToken : Token
    {
        public Color Value => (Color)(_value ?? throw new InvalidOperationException($"ForegroundStartToken should have a value"));
        public ForegroundStartToken(Color color) : base(color) { }
    }
    public class BackgroundStartToken : Token
    {
        public Color Value => (Color)(_value ?? throw new InvalidOperationException($"BackgroundStartToken should have a value"));
        public BackgroundStartToken(Color color) : base(color) { }
    }
    public class ForegroundStopToken : Token
    {
        public ForegroundStopToken() : base(null) { }
    }
    public class BackgroundStopToken : Token
    {
        public BackgroundStopToken() : base(null) { }
    }
    public class StyleStartToken : Token
    {
        public FontStyle Value => (FontStyle)(_value ?? throw new InvalidOperationException($"StyleStartToken should have a value"));
        public StyleStartToken(FontStyle style) : base(style) { }
    }
    public class StyleStopToken : Token
    {
        public FontStyle Value => (FontStyle)(_value ?? throw new InvalidOperationException($"StyleStopToken should have a value"));
        public StyleStopToken(FontStyle style) : base(style) { }
    }
    public class ResetAllToken : Token
    {
        public ResetAllToken() : base(null) { }
    }
}

internal enum ParserState
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
    StartItalic,
    StartUnderline,
    StopForeground,
    StopBackground,
    StopBold,
    StopItalic,
    StopUnderline
}