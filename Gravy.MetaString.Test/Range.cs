namespace Gravy.MetaString.Test;

public class Range
{
    private readonly MetaString<int> _simple = "Hello World".Meta(0);
    private readonly MetaString<int> _complex = "He".Meta(1) + "ll".Meta(2) + "o " + "Wor".Meta(3) + "ld";
        
    [Test] public void Simple_Start_Offset_Text() => Assert.That(_simple[2..].RawText, Is.EqualTo("llo World"));
    [Test] public void Complex_Start_Offset_Text() => Assert.That(_complex[2..].RawText, Is.EqualTo("llo World"));
    [Test] public void Complex_Start_Offset_Meta() => Assert.That(_complex[2..].OffsetInts(), Is.EqualTo(new [] {(0, 2), (2, 0), (4, 3), (7, 0)}));
        
    [Test] public void Simple_StartEnd_Offset_Text() => Assert.That(_simple[1..8].RawText, Is.EqualTo("ello Wo"));
    [Test] public void Complex_StartEnd_Offset_Text() => Assert.That(_complex[1..8].RawText, Is.EqualTo("ello Wo"));
    [Test] public void Complex_StartEnd_Offset_Meta() => Assert.That(_complex[1..8].OffsetInts(), Is.EqualTo(new [] {(0, 1), (1, 2), (3, 0), (5, 3)}));

    [Test] public void Simple_End_Offset_Text() => Assert.That(_simple[..8].RawText, Is.EqualTo("Hello Wo"));
    [Test] public void Complex_End_Offset_Text() => Assert.That(_complex[..8].RawText, Is.EqualTo("Hello Wo"));
    [Test] public void Complex_End_Offset_Meta() => Assert.That(_complex[..8].OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 2), (4, 0), (6, 3)}));

    [Test] public void Simple_FromEnd_End_Offset_Text() => Assert.That(_simple[..^2].RawText, Is.EqualTo("Hello Wor"));
    [Test] public void Complex_FromEnd_End_Offset_Text() => Assert.That(_complex[..^2].RawText, Is.EqualTo("Hello Wor"));
    [Test] public void Complex_FromEnd_End_Offset_Meta() => Assert.That(_complex[..^2].OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 2), (4, 0), (6, 3)}));
        
    [Test] public void Simple_FromEnd_StartEnd_Offset_Text() => Assert.That(_simple[^8..^2].RawText, Is.EqualTo("lo Wor"));
    [Test] public void Complex_FromEnd_StartEnd_Offset_Text() => Assert.That(_complex[^8..^2].RawText, Is.EqualTo("lo Wor"));
    [Test] public void Complex_FromEnd_StartEnd_Offset_Meta() => Assert.That(_complex[^8..^2].OffsetInts(), Is.EqualTo(new [] {(0, 2), (1, 0), (3, 3)}));

    [Test] public void Simple_FromEnd_Outside_Range() => Assert.That(() => _simple[^20..], Throws.TypeOf<IndexOutOfRangeException>());
    [Test] public void Complex_FromEnd_Outside_Range() => Assert.That(() => _complex[^20..], Throws.TypeOf<IndexOutOfRangeException>());

    [Test] public void Simple_Outside_Range() => Assert.That(() => _simple[..20], Throws.TypeOf<IndexOutOfRangeException>());
    [Test] public void Complex_Outside_Range() => Assert.That(() => _complex[..20], Throws.TypeOf<IndexOutOfRangeException>());

    [Test] public void Simple_InvalidRange() => Assert.That(() => _simple[2..1], Throws.ArgumentException);
    [Test] public void Complex_InvalidRange() => Assert.That(() => _complex[2..1], Throws.ArgumentException);
}