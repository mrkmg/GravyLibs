using System.Drawing;

namespace Gravy.ConsoleString.Test;

public class Tags
{
    private static readonly Color Red = Color.Red;
    private static readonly Color Blue = Color.Blue;
    private static readonly Color Green = Color.Green;

    [Test]
    public void Parse()
    {
        var parse1 = ConsoleString.Parse(@"[B]Hello[/B]\\[I]World[/I]");
        var parse2 = ConsoleString.Parse(@"[B][F#FF0000]He[/F]llo[//] [I][G!Green]Wo[/I]rld[/G]");
        var parse3 = ConsoleString.Parse(@"[B][F!Red][I]Hel[F!Blue]lo[//] [U]World");
        
        var proc1 = "Hello".CS().WithBold() + @"\" + "World".CS().WithItalic();
        var proc2 = "He".CS().WithBold().WithForeground(Red) +
                    "llo".CS().WithBold() +
                    " " +
                    "Wo".CS().WithItalic().WithBackground(Green) +
                    "rld".CS().WithBackground(Green);
        var proc3 = "Hel".CS().WithBold().WithItalic().WithForeground(Red) +
                    "lo".CS().WithBold().WithItalic().WithForeground(Blue) +
                    " " +
                    "World".CS().WithUnderline();

        Assert.Multiple(() =>
        {
            Assert.That(parse1, Is.EqualTo(proc1));
            Assert.That(parse2, Is.EqualTo(proc2));
            Assert.That(parse3, Is.EqualTo(proc3));
        });
    }

    [Test]
    public void ToTags()
    {
        var cStr1 = "Hello".CS().WithBold() + @"\" + "World".CS().WithItalic();
        var cStr2 = "He".CS().WithBold().WithForeground(Red) +
                    "llo".CS().WithBold() +
                    " " +
                    "Wo".CS().WithItalic().WithBackground(Green) +
                    "rld".CS().WithBackground(Green);
        var cStr3 = "Hel".CS().WithBold().WithItalic().WithForeground(Color.FromArgb(0xFF0000)) +
                    "lo".CS().WithBold().WithItalic().WithForeground(Blue) +
                    " " +
                    "World".CS().WithUnderline();
        Assert.Multiple(() =>
        {
            Assert.That(cStr1.ToTaggedString(), Is.EqualTo("[B]Hello[/B]\\[I]World[/I]"));
            Assert.That(cStr1.ToTaggedString(true), Is.EqualTo("[B]Hello[//]\\[I]World[//]"));
            
            Assert.That(cStr2.ToTaggedString(), Is.EqualTo("[F!Red][B]He[/F]llo[/B] [G!Green][I]Wo[/I]rld[/G]"));
            Assert.That(cStr2.ToTaggedString(true), Is.EqualTo("[F!Red][B]He[/F]llo[//] [G!Green][I]Wo[/I]rld[//]"));
            
            Assert.That(cStr3.ToTaggedString(), Is.EqualTo("[F#FF0000][B][I]Hel[F!Blue]lo[/F][/B][/I] [U]World[/U]"));
            Assert.That(cStr3.ToTaggedString(true), Is.EqualTo("[F#FF0000][B][I]Hel[F!Blue]lo[//] [U]World[//]"));
        });
    }
}