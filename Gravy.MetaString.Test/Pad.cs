namespace Gravy.MetaString.Test;

public class Pad
{
    private readonly MetaString<int> _simple = "Hello".Meta(0);
    private readonly MetaString<int> _complex = "Hello".Meta(1) + " " + "World".Meta(2);
        
    [Test] public void Simple_PadLeft() => Assert.That(_simple.PadLeft(10).RawText, Is.EqualTo("     Hello"));
    [Test] public void Simple_PadRight() => Assert.That(_simple.PadRight(10).RawText, Is.EqualTo("Hello     "));
        
    [Test] public void Simple_PadLeft_Noop() => Assert.That(_simple.PadLeft(5).RawText, Is.EqualTo("Hello"));
    [Test] public void Simple_PadRight_Noop() => Assert.That(_simple.PadRight(5).RawText, Is.EqualTo("Hello"));

    [Test] public void Simple_PadLeft_AltChar() => Assert.That(_simple.PadLeft('_', 10).RawText, Is.EqualTo("_____Hello"));
    [Test] public void Simple_PadRight_AltChar() => Assert.That(_simple.PadRight('_', 10).RawText, Is.EqualTo("Hello_____"));
        
    [Test] public void Complex_PadLeft_Text() => Assert.That(_complex.PadLeft(20).RawText, Is.EqualTo("         Hello World"));
    [Test] public void Complex_PadLeft_Meta() => Assert.That(_complex.PadLeft(20).OffsetInts(), Is.EqualTo(new [] { (0, 0), (9, 1), (14, 0), (15, 2) }));
    [Test] public void Complex_PadRight_Text() => Assert.That(_complex.PadRight(20).RawText, Is.EqualTo("Hello World         "));
    [Test] public void Complex_PadRight_Meta() => Assert.That(_complex.PadRight(20).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2), (11, 0) }));
        
    [Test] public void Complex_PadLeft_Noop_Text() => Assert.That(_complex.PadLeft(10).RawText, Is.EqualTo("Hello World"));
    [Test] public void Complex_PadLeft_Noop_Meta() => Assert.That(_complex.PadLeft(10).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2) }));
    [Test] public void Complex_PadRight_Noop_Text() => Assert.That(_complex.PadRight(10).RawText, Is.EqualTo("Hello World"));
    [Test] public void Complex_PadRight_Noop_Meta() => Assert.That(_complex.PadRight(10).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2) }));
        
    [Test] public void Complex_PadLeft_AltChar_Text() => Assert.That(_complex.PadLeft('_', 20).RawText, Is.EqualTo("_________Hello World"));
    [Test] public void Complex_PadLeft_AltChar_Meta() => Assert.That(_complex.PadLeft('_', 20).OffsetInts(), Is.EqualTo(new [] { (0, 0), (9, 1), (14, 0), (15, 2) }));
    [Test] public void Complex_PadRight_AltChar_Text() => Assert.That(_complex.PadRight('_', 20).RawText, Is.EqualTo("Hello World_________"));
    [Test] public void Complex_PadRight_AltChar_Meta() => Assert.That(_complex.PadRight('_', 20).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2), (11, 0) }));
}