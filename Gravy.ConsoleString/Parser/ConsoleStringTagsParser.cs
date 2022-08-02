using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Gravy.MetaString;

namespace Gravy.ConsoleString.Parser;

internal class ConsoleStringTagsParser
{
    // Allow inconsistent naming because the parser has no public properties or fields.
    // ReSharper disable InconsistentNaming
    private readonly string SourceText;
    private readonly List<MetaEntry<ConsoleFormat>> Entries = new();
    private int Index;
    private readonly List<char> Buffer = new();
    private ParserState State = ParserState.Text;
    private ConsoleFormat Format = ConsoleFormat.Default;
    // ReSharper restore InconsistentNaming
        
    private char Char => SourceText[Index];
    private string Text => new(Buffer.ToArray());
    private MetaEntry<ConsoleFormat> CurrentEntry => new(Text, Format);

    public ConsoleStringTagsParser(string sourceText)
    {
        SourceText = sourceText;
    }

    public ConsoleString Parse()
    {
        while (Index < SourceText.Length)
        {
            ParseNext();
        }
        if (State != ParserState.Text)
            throw new UnexpectedEndOfStringException(Index - 1);
        if (Buffer.Count > 0)
            Entries.Add(CurrentEntry);
        return new (Entries);
    }

    private void ParseNext()
    {
        switch (State)
        {
            case ParserState.Text:
                switch (Char)
                {
                    case '\\':
                        Index++;
                        break;
                    case '[':
                        if (Buffer.Count > 0)
                        {
                            Entries.Add(CurrentEntry);
                            Buffer.Clear();
                        }
                            
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
                        return;
                }
                Buffer.Add(Char);
                Index++;
                return;
            case ParserState.ParseStartTag:
                ParseStartTag();
                Index++;
                return;
            case ParserState.ParseStopTag:
                ParseStopTag();
                Index++;
                return;
            case ParserState.ResetAll:
                Format = ConsoleFormat.Default;
                VerifyCloseBracketAndProgressIndex();
                return;
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
                Format = Format.WithForeground(ReadColorHexValue(SourceText[Index..NextCloseBracketIndex()]));
                Index = NextCloseBracketIndex() + 1;
                State = ParserState.Text;
                break;
            case ParserState.ParseBackgroundHex:
                Format = Format.WithBackground(ReadColorHexValue(SourceText[Index..NextCloseBracketIndex()]));
                Index = NextCloseBracketIndex() + 1;
                State = ParserState.Text;
                break;
            case ParserState.ParseForegroundName:
                Format = Format.WithForeground(ReadColorNameValue(SourceText[Index..NextCloseBracketIndex()]));
                Index = NextCloseBracketIndex() + 1;
                State = ParserState.Text;
                break;
            case ParserState.ParseBackgroundName:
                Format = Format.WithBackground(ReadColorNameValue(SourceText[Index..NextCloseBracketIndex()]));
                Index = NextCloseBracketIndex() + 1;
                State = ParserState.Text;
                break;
            case ParserState.StartBold:
                Format = Format.WithBold();
                VerifyCloseBracketAndProgressIndex();
                break;
            case ParserState.StartItalic:
                Format = Format.WithItalic();
                VerifyCloseBracketAndProgressIndex();
                break;
            case ParserState.StartUnderline:
                Format = Format.WithUnderline();
                VerifyCloseBracketAndProgressIndex();
                break;
            case ParserState.StopForeground:
                Format = Format.WithForeground(null);
                VerifyCloseBracketAndProgressIndex();
                break;
            case ParserState.StopBackground:
                Format = Format.WithBackground(null);
                VerifyCloseBracketAndProgressIndex();
                break;
            case ParserState.StopBold:
                Format = Format.WithoutBold();
                VerifyCloseBracketAndProgressIndex();
                break;
            case ParserState.StopItalic:
                Format = Format.WithoutItalic();
                VerifyCloseBracketAndProgressIndex();
                break;
            case ParserState.StopUnderline:
                Format = Format.WithoutUnderline();
                VerifyCloseBracketAndProgressIndex();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
        
    private enum ParserState
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

}