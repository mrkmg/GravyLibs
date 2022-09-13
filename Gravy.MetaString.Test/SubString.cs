namespace Gravy.MetaString.Test;

public class SubString
{
    private readonly MetaString<int> _simple = "Hello World".Meta(0);
    private readonly MetaString<int> _complex = "Hello".Meta(1) + " " + "World".Meta(2);

    [Test] public void Throws_Invalid_Range() => Assert.That(() => _simple.Substring(1, -1), Throws.TypeOf<ArgumentOutOfRangeException>());
    [Test] public void Simple_Single_OnlySize() => Assert.That(_simple.Substring(3).RawText, Is.EqualTo("lo World"));
    [Test] public void Complex_Single_OnlySize() => Assert.That(_complex.Substring(3).RawText, Is.EqualTo("lo World"));
    [Test] public void Complex_Single_OnlySize_Meta() => Assert.That(_complex.Substring(3).OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 0), (3, 2)}));
            
    [Test] public void Simple_SizeAndLength() => Assert.That(_simple.Substring(3, 6).RawText, Is.EqualTo("lo Wo"));
    [Test] public void Complex_SizeAndLength() => Assert.That(_complex.Substring(3, 6).RawText, Is.EqualTo("lo Wo"));
    [Test] public void Complex_SizeAndLength_Meta() => Assert.That(_complex.Substring(3, 6).OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 0), (3, 2)}));
}