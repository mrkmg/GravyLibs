using System.Drawing;
using Gravy.ConsoleString.Tags;
using NUnit.Framework.Constraints;
// ReSharper disable MemberCanBePrivate.Global

namespace Gravy.ConsoleString.Test;

public class Tags
{
    private static readonly Color Red = Color.Red;
    private static readonly Color Blue = Color.Blue;
    private static readonly Color Green = Color.Green;


    public class Compiler
    {

        [Test] public void NoTags() 
            => "This\n is\r\n a simple\r string".WhenParsed(Is.EqualTo("This\n is\r\n a simple\r string".CS()));

        [Test] public void ResetThrowsInStrict() 
            => "This\n is\r\n a sim[//]ple\r string".WhenParsedStrictly(
                Throws.TypeOf<ResetAllInStrictModeException>()
                    .With.Property("Line").EqualTo(3)
                    .And.Property("Column").EqualTo(7));
        
        [Test] public void ResetDoesNotThrowInNonStrict()
            => "This\n is\r\n a sim[//]ple\r string".WhenParsed(Throws.Nothing);
        
        [Test] public void UnfinishedTagThrows()
            => "[B]Test[/B".WhenParsed(Throws.TypeOf<UnexpectedEndOfStringException>());
        
        [Test] public void WithEscapedCharacters() 
            => @"This is a string with escaped characters: \\ \[".WhenParsed(Is.EqualTo(new ConsoleString(@"This is a string with escaped characters: \ [")));
        
        [Test] public void InvalidHexColorLengthThrows()
            => @"[F#FF]Test[/F]".WhenParsed(Throws.TypeOf<InvalidHexColorException>());
        
        [Test] public void InvalidHexColorLettersThrows()
            => @"[F#FFFFGG]Test[/F]".WhenParsed(Throws.TypeOf<InvalidHexColorException>());
        
        [Test] public void UnknownColorNameThrows()
            => @"[F!Funky]Test[/F]".WhenParsed(Throws.TypeOf<UnknownNamedColorException>());
        
        [Test] public void WithAllTags() 
            => ("Text " +
                @"[B]Bold[/B] [L]Light[/L] " +
                @"[I]Italic[/I] [U]Underline[/U] [T]StrikeThrough[/T] [K]Blink[/K] [V]Inverse[/V] " +
                @"[F#FF0000]Foreground[/F] [G!Green]Background[/G] [B][I][U][F!Red][G#00FF00]All[//] None")
                .WhenParsed(Is.EqualTo("Text " +
                                       "Bold".CS().WithBold() + " " +
                                       "Light".CS().WithLight() + " " +
                                       "Italic".CS().WithItalic() + " " +
                                       "Underline".CS().WithUnderline() + " " +
                                       "StrikeThrough".CS().WithStrikeThrough() + " " +
                                       "Blink".CS().WithBlink() + " " +
                                       "Inverse".CS().WithInverse() + " " +
                                       "Foreground".CS().WithForeground(Color.Red) + " " +
                                       "Background".CS().WithBackground(Color.Green) + " " +
                                       "All".CS().WithBold().WithItalic().WithUnderline()
                                           .WithForeground(Color.Red).WithBackground(Color.FromArgb(0, 255, 0)) + " " +
                                       "None".CS()));
        
        [Test] public void WithUnmatchedStyleStartTag()
            => @"[B]Test".WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStartTokenException>());
        
        [Test] public void WithUnmatchedForegroundStartTag()
            => @"[F!Red]Test".WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStartTokenException>());
        
        [Test] public void WithUnmatchedBackgroundStartTag()
            => @"[G!Red]Test".WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStartTokenException>());
        
        [Test] public void WithUnmatchedStyleEndTag()
            => @"Test[/B]".WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStopTokenException>());
        
        [Test] public void WithUnmatchedForegroundEndTag()
            => @"Test[/F]".WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStopTokenException>());
        
        [Test] public void WithUnmatchedBackgroundEndTag()
            => @"Test[/G]".WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStopTokenException>());
        
        [Test] public void WithDuplicateStyleTags()
            => @"[I][I]Test[/I]Test[/I]".WhenParsedStrictly(Throws.Exception.TypeOf<DuplicateStyleException>());
        
        [Test] public void WithMissingBracketOnStartTag()
            => @"[BTesting[/B] Testing".WhenParsed(Throws.Exception.TypeOf<MissingCloseBracketException>());
        
        [Test] public void WithMissingBracketOnEndTag()
            => @"[B]Testing[/B Testing".WhenParsed(Throws.Exception.TypeOf<MissingCloseBracketException>());
        
        [Test] public void WithMissingBracketOnColorTag()
            => @"[F!RedTesting".WhenParsed(Throws.Exception.TypeOf<UnexpectedEndOfStringException>());
        
        [Test] public void InvalidForegroundColorType()
            => @"[F^Other]Test".WhenParsed(Throws.Exception.TypeOf<InvalidColorParserException>());
        
        [Test] public void InvalidBackgroundColorType()
            => @"[G^Other]Test".WhenParsed(Throws.Exception.TypeOf<InvalidColorParserException>());

        [Test] public void UnknownStartTag()
            => @"[Q]Testing".WhenParsed(Throws.Exception.TypeOf<UnknownTagException>());
        
        [Test] public void UnknownEndTag()
            => @"Testing[/Q]".WhenParsed(Throws.Exception.TypeOf<UnknownTagException>());
        
        [Test] public void WithValidDuplicateForegroundTags()
            => @"[F!Red][F!Blue]Test[/F]Test[/F]".WhenParsedStrictly(Throws.Nothing);
        
        [Test] public void WithInvalidDuplicateForegroundTags()
            => @"[F!Red][F!Red]Test[/F]Test[/F]".WhenParsedStrictly(Throws.Exception.TypeOf<DuplicateColorException>());
        
        [Test] public void WithValidDuplicateBackgroundTags()
            => @"[G#00FF00][G#0000FF]Test[/G]Test[/G]".WhenParsedStrictly(Throws.Nothing);
        
        [Test] public void WithInvalidDuplicateBackgroundTags()
            => @"[G#00FF00][G#00FF00]Test[/G]Test[/G]".WhenParsedStrictly(Throws.Exception.TypeOf<DuplicateColorException>());
    }
    
    public class ToTags
    {
        public ConsoleString BoldEscape = "Hello".CS().WithBold() + @"\" + "World".CS().WithItalic();
        public ConsoleString Overlapping = "He".CS().WithBold().WithForeground(Red) +
                    "llo".CS().WithBold() +
                    " " +
                    "Wo".CS().WithItalic().WithBackground(Green) +
                    "rld".CS().WithBackground(Green);
        public ConsoleString ColorVariety = "Hel".CS().WithBold().WithItalic().WithForeground(Color.FromArgb(0xFF0000)) +
                    "lo".CS().WithBold().WithItalic().WithForeground(Blue) +
                    " " +
                    "World".CS().WithUnderline();

        [Test] public void BoldAndEscape() => BoldEscape.WhenTagged(Is.EqualTo("[B]Hello[/B]\\[I]World[/I]"));
        [Test] public void BoldAndEscapeWithReset() => BoldEscape.WhenTaggedWithReset(Is.EqualTo("[B]Hello[//]\\[I]World[//]"));
        [Test] public void OverlappingStyle() => Overlapping.WhenTagged(Is.EqualTo("[F!Red][B]He[/F]llo[/B] [G!Green][I]Wo[/I]rld[/G]"));
        [Test] public void OverlappingStyleWithReset() => Overlapping.WhenTaggedWithReset(Is.EqualTo("[F!Red][B]He[/F]llo[//] [G!Green][I]Wo[/I]rld[//]"));
        [Test] public void ColorVarieties() => ColorVariety.WhenTagged(Is.EqualTo("[F#FF0000][B][I]Hel[F!Blue]lo[/F][/B][/I] [U]World[/U]"));
        [Test] public void ColorVarietiesWithReset() => ColorVariety.WhenTaggedWithReset(Is.EqualTo("[F#FF0000][B][I]Hel[F!Blue]lo[//] [U]World[//]"));
    }
}

public static class TagsTestExtension
{
    public static void WhenParsed(this string taggedString, IResolveConstraint c)
        => Assert.That(() => ConsoleString.Parse(taggedString), c);
    
    public static void WhenParsedStrictly(this string taggedString, IResolveConstraint c)
        => Assert.That(() => ConsoleString.Parse(taggedString, true), c);
    
    public static void WhenTagged(this ConsoleString consoleString, IResolveConstraint c)
        => Assert.That(() => consoleString.ToTaggedString(), c);
    
    public static void WhenTaggedWithReset(this ConsoleString consoleString, IResolveConstraint c)
        => Assert.That(() => consoleString.ToTaggedString(true), c);
}