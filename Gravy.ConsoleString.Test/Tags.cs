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
        public const string TaggedStringWithNoTags = "This\n is\r\n a simple\r string";
        public const string TaggedStringWithReset = "This\n is\r\n a sim[//]ple\r string";
        public const string TaggedStringUnfinishedTag = "[B]Test[/B";
        public const string TaggedStringWithEscapedCharacters = @"This is a string with escaped characters: \\ \[";
        public const string TaggedStringWithInvalidHexColorLength = @"[F#FF]Test[/F]";
        public const string TaggedStringWithInvalidHexColorLetters = @"[F#FFFFGG]Test[/F]";
        public const string TaggedStringWithUnknownColorName = @"[F!Funky]Test[/F]";
        public const string TaggedStringWithAllTags = @"[B]Bold[/B] [L]Light[/L] [I]Italic[/I] [U]Underline[/U] [T]StrikeThrough[/T] [K]Blink[/K] [V]Inverse[/V] [F!Red]Foreground[/F] [G#00FF00]Background[/G] [B][I][U][F!Red][G#00FF00]All[//] None";
        public const string TaggedStringWithUnmatchedStyleStartTag = @"[B]Test";
        public const string TaggedStringWithUnmatchedForegroundStartTag = @"[F!Red]Test";
        public const string TaggedStringWithUnmatchedBackgroundStartTag = @"[G!Red]Test";
        public const string TaggedStringWithUnmatchedStyleEndTag = @"Test[/B]";
        public const string TaggedStringWithUnmatchedForegroundEndTag = @"Test[/F]";
        public const string TaggedStringWithUnmatchedBackgroundEndTag = @"Test[/G]";
        public const string TaggedStringWithDuplicateWeightTags = @"[B][B]Test[/B]Test[/B]";
        public const string TaggedStringWithDuplicateStyleTags = @"[I][I]Test[/I]Test[/I]";
        public const string TaggedStringWithValidDuplicateForegroundTags = @"[F!Red][F!Blue]Test[/F]Test[/F]";
        public const string TaggedStringWithInvalidDuplicateForegroundTags = @"[F!Red][F!Red]Test[/F]Test[/F]";
        public const string TaggedStringWithValidDuplicateBackgroundTags = @"[G#00FF00][G#0000FF]Test[/G]Test[/G]";
        public const string TaggedStringWithInvalidDuplicateBackgroundTags = @"[G#00FF00][G#00FF00]Test[/G]Test[/G]";

        public readonly ConsoleString ExpectedStringWithNoTags = new("This\n is\r\n a simple\r string");
        public readonly ConsoleString ExpectedStringWithEscapedCharacters = new(@"This is a string with escaped characters: \ [");

        public readonly ConsoleString ExpectedStringWithAllTags = "Bold".CS().WithBold() + " " +
                                                                  "Light".CS().WithLight() + " " +
                                                                  "Italic".CS().WithItalic() + " " +
                                                                  "Underline".CS().WithUnderline() + " " +
                                                                  "StrikeThrough".CS().WithStrikeThrough() + " " +
                                                                  "Blink".CS().WithBlink() + " " +
                                                                  "Inverse".CS().WithInverse() + " " +
                                                                  "Foreground".CS().WithForeground(Color.Red) + " " +
                                                                  "Background".CS().WithBackground(Color.FromArgb(0, 255, 0)) + " " +
                                                                  "All".CS().WithBold().WithItalic().WithUnderline()
                                                                      .WithForeground(Color.Red).WithBackground(Color.FromArgb(0, 255, 0)) + " " +
                                                                  "None".CS();

        [Test] public void NoTags() 
            => TaggedStringWithNoTags.WhenParsed(Is.EqualTo(ExpectedStringWithNoTags));

        [Test] public void ResetThrowsInStrict() 
            => TaggedStringWithReset.WhenParsedStrictly(
                Throws.TypeOf<ResetAllInStrictModeException>()
                    .With.Property("Line").EqualTo(3)
                    .And.Property("Column").EqualTo(7));
        
        [Test] public void ResetDoesNotThrowInNonStrict()
            => TaggedStringWithReset.WhenParsed(Throws.Nothing);
        
        [Test] public void UnfinishedTagThrows()
            => TaggedStringUnfinishedTag.WhenParsed(Throws.TypeOf<UnexpectedEndOfStringException>());
        
        [Test] public void WithEscapedCharacters() 
            => TaggedStringWithEscapedCharacters.WhenParsed(Is.EqualTo(ExpectedStringWithEscapedCharacters));
        
        [Test] public void InvalidHexColorLengthThrows()
            => TaggedStringWithInvalidHexColorLength.WhenParsed(Throws.TypeOf<InvalidHexColorException>());
        
        [Test] public void InvalidHexColorLettersThrows()
            => TaggedStringWithInvalidHexColorLetters.WhenParsed(Throws.TypeOf<InvalidHexColorException>());
        
        [Test] public void UnknownColorNameThrows()
            => TaggedStringWithUnknownColorName.WhenParsed(Throws.TypeOf<UnknownNamedColorException>());
        
        [Test] public void WithAllTags() 
            => TaggedStringWithAllTags.WhenParsed(Is.EqualTo(ExpectedStringWithAllTags));
        
        [Test] public void WithUnmatchedStyleStartTag()
            => TaggedStringWithUnmatchedStyleStartTag.WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStartTokenException>());
        
        [Test] public void WithUnmatchedForegroundStartTag()
            => TaggedStringWithUnmatchedForegroundStartTag.WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStartTokenException>());
        
        [Test] public void WithUnmatchedBackgroundStartTag()
            => TaggedStringWithUnmatchedBackgroundStartTag.WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStartTokenException>());
        
        [Test] public void WithUnmatchedStyleEndTag()
            => TaggedStringWithUnmatchedStyleEndTag.WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStopTokenException>());
        
        [Test] public void WithUnmatchedForegroundEndTag()
            => TaggedStringWithUnmatchedForegroundEndTag.WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStopTokenException>());
        
        [Test] public void WithUnmatchedBackgroundEndTag()
            => TaggedStringWithUnmatchedBackgroundEndTag.WhenParsedStrictly(Throws.Exception.TypeOf<UnmatchedStopTokenException>());
        
        [Test] public void WithDuplicateStyleTags()
            => TaggedStringWithDuplicateStyleTags.WhenParsedStrictly(Throws.Exception.TypeOf<DuplicateStyleException>());
        
        [Test] public void WithValidDuplicateForegroundTags()
            => TaggedStringWithValidDuplicateForegroundTags.WhenParsedStrictly(Throws.Nothing);
        
        [Test] public void WithInvalidDuplicateForegroundTags()
            => TaggedStringWithInvalidDuplicateForegroundTags.WhenParsedStrictly(Throws.Exception.TypeOf<DuplicateColorException>());
        
        [Test] public void WithValidDuplicateBackgroundTags()
            => TaggedStringWithValidDuplicateBackgroundTags.WhenParsedStrictly(Throws.Nothing);
        
        [Test] public void WithInvalidDuplicateBackgroundTags()
            => TaggedStringWithInvalidDuplicateBackgroundTags.WhenParsedStrictly(Throws.Exception.TypeOf<DuplicateColorException>());
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