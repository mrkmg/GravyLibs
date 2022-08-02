using System.Drawing;

namespace Gravy.ConsoleString.Test;

public class StringCompat
{
    private static readonly Color Red = Color.Red;
    private static readonly Color Blue = Color.Blue;
    private static readonly Color Green = Color.Green;
    
    [Test]
    public void Range()
    {
        var simpleStr = new ConsoleString("Hello World");
        var complexStr = new ConsoleString("He", Red) +
                         new ConsoleString("ll", Green) +
                         "o " +
                         new ConsoleString("Wor", Blue) +
                         "ld";

        Assert.Multiple(() =>
        {
            Assert.That(simpleStr[2..].RawText, Is.EqualTo("llo World"));
            Assert.That(complexStr[2..].RawText, Is.EqualTo("llo World"));
            Assert.That(complexStr[2..].ForegroundColors(), Is.EqualTo(new Color?[] {Green, null, Blue, null}));
            
            Assert.That(simpleStr[1..8].RawText, Is.EqualTo("ello Wo"));
            Assert.That(complexStr[1..8].RawText, Is.EqualTo("ello Wo"));
            Assert.That(complexStr[1..8].ForegroundColors(), Is.EqualTo(new Color?[] {Red, Green, null, Blue}));
            
            Assert.That(simpleStr[..8].RawText, Is.EqualTo("Hello Wo"));
            Assert.That(complexStr[..8].RawText, Is.EqualTo("Hello Wo"));
            Assert.That(complexStr[..8].ForegroundColors(), Is.EqualTo(new Color?[] {Red, Green, null, Blue}));
            
            Assert.That(simpleStr[..^2].RawText, Is.EqualTo("Hello Wor"));
            Assert.That(complexStr[..^2].RawText, Is.EqualTo("Hello Wor"));
            Assert.That(complexStr[..^2].ForegroundColors(), Is.EqualTo(new Color?[] {Red, Green, null, Blue}));
            
            Assert.That(simpleStr[^8..^2].RawText, Is.EqualTo("lo Wor"));
            Assert.That(complexStr[^8..^2].RawText, Is.EqualTo("lo Wor"));
            Assert.That(complexStr[^8..^2].ForegroundColors(), Is.EqualTo(new Color?[] {Green, null, Blue}));
            
            Assert.That(() => simpleStr[^20..], Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(() => complexStr[^20..], Throws.TypeOf<IndexOutOfRangeException>());
            
            Assert.That(() => simpleStr[..20], Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(() => complexStr[..20], Throws.TypeOf<IndexOutOfRangeException>());
            
            Assert.That(() => simpleStr[2..1], Throws.ArgumentException);
            Assert.That(() => simpleStr[2..1], Throws.ArgumentException);

        });
    }

    [Test]
    public void Pad()
    {
        var cs = "Hello".CS();
        var cs2 = "[F!Red]Hello[/F] [F!Blue]World[/F]".CS();
        
        Assert.Multiple(() =>
        {
            Assert.That(cs.PadLeft(5).RawText, Is.EqualTo("Hello"));
            Assert.That(cs.PadRight(5).RawText, Is.EqualTo("Hello"));
            
            Assert.That(cs.PadLeft(10).RawText, Is.EqualTo("     Hello"));
            Assert.That(cs.PadRight(10).RawText, Is.EqualTo("Hello     "));
            
            Assert.That(cs.PadLeft('_', 10).RawText, Is.EqualTo("_____Hello"));
            Assert.That(cs.PadRight('_', 10).RawText, Is.EqualTo("Hello_____"));
            
            Assert.That(cs2.PadLeft(10).RawText, Is.EqualTo("Hello World"));
            Assert.That(cs2.PadLeft(10).ForegroundColors(), Is.EqualTo(new Color?[] { Red, null, Blue }));
            Assert.That(cs2.PadRight(10).RawText, Is.EqualTo("Hello World"));
            Assert.That(cs2.PadRight(10).ForegroundColors(), Is.EqualTo(new Color?[] { Red, null, Blue }));
            
            Assert.That(cs2.PadLeft(20).RawText, Is.EqualTo("         Hello World"));
            Assert.That(cs2.PadLeft(20).ForegroundColors(), Is.EqualTo(new Color?[] { null, Red, null, Blue }));
            Assert.That(cs2.PadRight(20).RawText, Is.EqualTo("Hello World         "));
            Assert.That(cs2.PadRight(20).ForegroundColors(), Is.EqualTo(new Color?[] { Red, null, Blue, null }));
            
            Assert.That(cs2.PadLeft('_', 20).RawText, Is.EqualTo("_________Hello World"));
            Assert.That(cs2.PadLeft('_', 20).ForegroundColors(), Is.EqualTo(new Color?[] { null, Red, null, Blue }));
            Assert.That(cs2.PadRight('_', 20).RawText, Is.EqualTo("Hello World_________"));
            Assert.That(cs2.PadRight('_', 20).ForegroundColors(), Is.EqualTo(new Color?[] { Red, null, Blue, null }));
        });
    }

    [Test]
    public void Trim()
    {
        var simple1 = "   Hello World   ".CS();
        var simple2 = "___Hello World___".CS();
        var simple3 = "-_-Hello World-_-".CS();
        var complex1 = "   Hello".CS().FG(Blue) + " " + "World   ".CS().FG(Red);
        var complex2 = "___Hello".CS().FG(Blue) + " " + "World___".CS().FG(Red);
        var complex3 = "-_-Hello".CS().FG(Blue) + " " + "World-_-".CS().FG(Red);
        var fullRemove1 = " ".CS().FG(Blue) + " ".CS().FG(Red) +
                          "Hello World" +
                          " ".CS().FG(Blue) + " ".CS().FG(Red);
        
        Assert.Multiple(() =>
        {
            Assert.That(ConsoleString.Empty.Trim().RawText, Is.EqualTo(string.Empty));
            
            Assert.That(simple1.Trim().RawText, Is.EqualTo("Hello World"));
            Assert.That(simple2.Trim('_').RawText, Is.EqualTo("Hello World"));
            Assert.That(simple3.Trim('-','_').RawText, Is.EqualTo("Hello World"));
            
            Assert.That(complex1.Trim().RawText, Is.EqualTo("Hello World"));
            Assert.That(complex2.Trim('_').RawText, Is.EqualTo("Hello World"));
            Assert.That(complex3.Trim('-','_').RawText, Is.EqualTo("Hello World"));
            
            Assert.That(fullRemove1.Trim().RawText, Is.EqualTo("Hello World"));
        });
    }

    [Test]
    public void Substring()
    {
        var simple1 = "Hello World".CS();
        var complex1 = "Hello".CS().FG(Blue) + " " + "World".CS().FG(Red);
        Assert.Multiple(() =>
        {
            Assert.That(() => simple1.Substring(1, -1), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(simple1.Substring(3).RawText, Is.EqualTo("lo World"));
            Assert.That(complex1.Substring(3).RawText, Is.EqualTo("lo World"));
            Assert.That(complex1.Substring(3).ForegroundColors(), Is.EqualTo(new Color?[] {Blue, null, Red}));
            
            Assert.That(simple1.Substring(3, 6).RawText, Is.EqualTo("lo Wo"));
            Assert.That(complex1.Substring(3, 6).RawText, Is.EqualTo("lo Wo"));
            Assert.That(complex1.Substring(3, 6).ForegroundColors(), Is.EqualTo(new Color?[] {Blue, null, Red}));
        });
    }

    [Test]
    public void Contains()
    {
        var simple1 = "Hello World".CS();
        var complex1 = "Hello".CS().FG(Blue) + " " + "World".CS().FG(Red);
        var partialGood1 = "Hello".CS().FG(Blue);
        var partialGood2 = " " + "World".CS().FG(Red);
        var partialGood3 = "lo".CS().FG(Blue) + " " + "Wo".CS().FG(Red);
        var partialBad1 = "abc".CS().FG(Blue);
        var partialBad2 = "Hello".CS().FG(Green);
        var partialBad3 = "lo".CS().FG(Blue) + " " + "Wo".CS().FG(Green);

        Assert.Multiple(() =>
        {
            Assert.That(simple1.Contains(string.Empty), Is.True, "Empty string should be contained in any ConsoleString");
            Assert.That(complex1.Contains(ConsoleString.Empty), Is.True, "Empty ConsoleString should be contained in any ConsoleString");
            
            Assert.That(simple1.Contains("lo Wor"), Is.True, "string should ignore format");
            Assert.That(complex1.Contains("lo Wor"), Is.True, "string should ignore format");
            
            Assert.That(simple1.Contains("abc"), Is.False, "should return false for non-existent string");
            Assert.That(complex1.Contains("abc"), Is.False, "should return false for non-existent string");
            
            Assert.That(complex1.Contains(partialBad1), Is.False, "should return false when text content is different");
            Assert.That(complex1.Contains(partialBad2), Is.False, "should return false when format is different (Single)");
            Assert.That(complex1.Contains(partialBad3), Is.False, "should return false when format is different (Multi)");
            
            Assert.That(complex1.Contains(partialGood1), Is.True, "should return true when text content and format is the same (single-start)");
            Assert.That(complex1.Contains(partialGood2), Is.True, "should return true when text content and format is the same (multi-end)");
            Assert.That(complex1.Contains(partialGood3), Is.True, "should return true when text content and format is the same (multi-middle)");
        });
    }

    [Test]
    public void Split()
    {
        var split1 = "[B]A[/B] [I]B[/I] [U]C[/U]".CS().Split(' ');
        var split2 = "[B]A[/B]  [I]B[/I]   [U]C[/U]".CS().Split(' ');
        var split3 = "[B]A[/B]  [I]B[/I]  [U]C[/U]".CS().Split(' ', 2);
        var split4 = "[B]A[/B]  [I]B[/I]  [U]C[/U]".CS().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var split5 = "[B]A[/B]  [I]B[/I]  [U]C[/U]".CS().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var split6 = "[B]A[/B]_[I]B[/I]_[U]C[/U]".CS().Split('_');
        var split7 = "[B]A[/B]_[I]B[/I]-[U]C[/U]".CS().Split(new [] {'_', '-'});
        var split8 = "[B]A[/B]_[I]B[/I]-[U]C[/U]".CS().Split(new [] {'_', '-'});
        var split9 = "[B]A[/B]TEST[I]B[/I]TEST[U]C[/U]".CS().Split("TEST");
        var split10 = "[B]A[/B]TEST[I]B[/I]test[U]C[/U]".CS().Split(new [] { "TEST", "test" });

        var result1 = new [] { "A".CS().WithBold(), "B".CS().WithItalic(), "C".CS().WithUnderline() };
        var result2 = new [] { "A".CS().WithBold(), ConsoleString.Empty, "B".CS().WithItalic(), ConsoleString.Empty, ConsoleString.Empty, "C".CS().WithUnderline() };
        var result3 = new [] { "A".CS().WithBold(), "B".CS().WithItalic() + "C".CS().WithUnderline() };
        var result4 = new [] { "A".CS().WithBold(), "B".CS().WithItalic(), "C".CS().WithUnderline() };
        var result5 = new [] { "A".CS().WithBold(), "B".CS().WithItalic() + "C".CS().WithUnderline() };

        if (split1[0] == result1[0])
        {
            
        }
        
        Assert.Multiple(() =>
        {
            Assert.That(split1, Is.EqualTo(result1));
            Assert.That(split2, Is.EqualTo(result2));
            Assert.That(split3, Is.EqualTo(result3));
            Assert.That(split4, Is.EqualTo(result4));
            Assert.That(split5, Is.EqualTo(result5));
            
            Assert.That(split6, Is.EqualTo(result1));
            Assert.That(split7, Is.EqualTo(result1));
            Assert.That(split8, Is.EqualTo(result1));
            Assert.That(split9, Is.EqualTo(result1));
            Assert.That(split10, Is.EqualTo(result1));
        });

    }
}