// ReSharper disable MemberCanBePrivate.Global
using System.Drawing;
using Gravy.Ansi;
using Gravy.ConsoleString.Tags;
using NUnit.Framework.Constraints;
using Gravy.MetaString;

namespace Gravy.ConsoleString.Test;

using ConsoleString = MetaString<ConsoleFormat>;

public class Tags
{
    public class IndividualTags
    {
        [Test] public void ForegroundSystemColor() => "[F!Red]Test[/F]".WhenParsed(Is.EqualTo("Test".With(Color.Red)));
        [Test] public void ForegroundThemeColor() => "[F@Blue]Test[/F]".WhenParsed(Is.EqualTo("Test".With(Ansi16Color.Blue)));
        [Test] public void ForegroundAnsi256Color() => "[F$Chartreuse4]Test[/F]".WhenParsed(Is.EqualTo("Test".With(Ansi256Color.Chartreuse4)));
        [Test] public void ForegroundRgbColor() => "[F#00FF00]Test[/F]".WhenParsed(Is.EqualTo("Test".With(Color.FromArgb(0, 255, 0))));
        
        [Test] public void BackgroundSystemColor() => "[G!Red]Test[/G]".WhenParsed(Is.EqualTo("Test".With(null, Color.Red)));
        [Test] public void BackgroundThemeColor() => "[G@Blue]Test[/G]".WhenParsed(Is.EqualTo("Test".With(null, Ansi16Color.Blue)));
        [Test] public void BackgroundAnsi256Color() => "[G$DarkSlateGray3]Test[/G]".WhenParsed(Is.EqualTo("Test".With(null, Ansi256Color.DarkSlateGray3)));
        [Test] public void BackgroundRgbColor() => "[G#00FF00]Test[/G]".WhenParsed(Is.EqualTo("Test".With(null, Color.FromArgb(0, 255, 0))));

        [Test] public void Bold() => "[B]Test[/B]".WhenParsed(Is.EqualTo("Test".With(FontWeight.Bold)));
        [Test] public void Light() => "[L]Test[/L]".WhenParsed(Is.EqualTo("Test".With(FontWeight.Light)));
        
        [Test] public void Italic() => "[I]Test[/I]".WhenParsed(Is.EqualTo("Test".With(FontStyle.Italic)));
        [Test] public void Underline() => "[U]Test[/U]".WhenParsed(Is.EqualTo("Test".With(FontStyle.Underline)));
        [Test] public void Strikethrough() => "[T]Test[/T]".WhenParsed(Is.EqualTo("Test".With(FontStyle.StrikeThrough)));
        [Test] public void Blink() => "[K]Test[/K]".WhenParsed(Is.EqualTo("Test".With(FontStyle.Blink)));
        [Test] public void Inverse() => "[V]Test[/V]".WhenParsed(Is.EqualTo("Test".With(FontStyle.Inverse)));
        
        [Test] public void WithAllTags() 
            => "[F!Red][G!Blue][B][I][U][T][K][V]Test[//]"
                .WhenParsed(Is.EqualTo("Test".With(
                    Color.Red, 
                    Color.Blue, 
                    FontWeight.Bold, 
                    FontStyle.Italic | FontStyle.Underline | FontStyle.StrikeThrough | FontStyle.Blink | FontStyle.Inverse)));
    }

    public class ErrorChecking
    {
        [Test] public void NoTags() 
            => "This\n is\r\n a simple\r string".WhenParsed(Is.EqualTo("This\n is\r\n a simple\r string".ParseTags()));

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
        
        [Test] public void UnknownThemeColorThrows()
            => @"[F@Funky]Test[/F]".WhenParsed(Throws.TypeOf<UnknownThemeColorException>());

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
        
        [Test] public void WithDuplicateWeightTags()
            => @"[B][B]Test[/B]Test[/B]".WhenParsedStrictly(Throws.Exception.TypeOf<DuplicateWeightException>());
        
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
        public ConsoleString BoldEscape = "Hello".ParseTags().WithBold() + @"\" + "World".ParseTags().WithItalic();
        public ConsoleString Overlapping = "He".ParseTags().WithBold().WithForeground(Color.Red) +
                    "llo".ParseTags().WithBold() +
                    " " +
                    "Wo".ParseTags().WithItalic().WithBackground(Color.Green) +
                    "rld".ParseTags().WithBackground(Color.Green);
        public ConsoleString ColorVariety = "Hel".ParseTags().WithBold().WithItalic().WithForeground(Color.FromArgb(255, 0, 0)) +
                    "lo".ParseTags().WithBold().WithItalic().WithForeground(Color.Blue) +
                    " " +
                    "World".ParseTags().WithUnderline();

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
        => Assert.That(() => taggedString.ParseTags(), c);
    
    public static void WhenParsedStrictly(this string taggedString, IResolveConstraint c)
        => Assert.That(() => taggedString.ParseTags(true), c);
    
    public static void WhenTagged(this ConsoleString consoleString, IResolveConstraint c)
        => Assert.That(() => consoleString.ToTaggedString(), c);
    
    public static void WhenTaggedWithReset(this ConsoleString consoleString, IResolveConstraint c)
        => Assert.That(() => consoleString.ToTaggedString(true), c);
}