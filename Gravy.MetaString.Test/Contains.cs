namespace Gravy.MetaString.Test;

public class Contains
{
    private readonly MetaString<int> _simple1 = "Hello World".Meta(0);
    private readonly MetaString<int> _complex1 = "Hello".Meta(1) + " " + "World".Meta(2);
    private readonly MetaString<int> _partialGood1 = "Hello".Meta(1);
    private readonly MetaString<int> _partialGood2 = " " + "World".Meta(2);
    private readonly MetaString<int> _partialGood3 = "lo".Meta(1) + " " + "Wo".Meta(2);
    private readonly MetaString<int> _partialBad1 = "abc".Meta(1);
    private readonly MetaString<int> _partialBad2 = "Hello".Meta(3);
    private readonly MetaString<int> _partialBad3 = "lo".Meta(1) + " " + "Wo".Meta(3);

    [Test] public void Contains_Empty_Simple() => Assert.That(_simple1.Contains(string.Empty), Is.True, "Empty string should be contained in any ConsoleString");
    [Test] public void Contains_Empty_Complex() => Assert.That(_complex1.Contains(MetaString<int>.Empty), Is.True, "Empty ConsoleString should be contained in any ConsoleString");
        
    [Test] public void Simple_Contains() => Assert.That(_simple1.Contains("lo Wor"), Is.True, "string should ignore meta");
    [Test] public void Complex_Contains() => Assert.That(_complex1.Contains("lo Wor"), Is.True, "string should ignore meta");
        
    [Test] public void Simple_Doesnt_Contain() => Assert.That(_simple1.Contains("abc"), Is.False, "should return false for non-existent string");
    [Test] public void Complex_Doesnt_Contain() => Assert.That(_complex1.Contains("abc"), Is.False, "should return false for non-existent string");
        
    [Test] public void Text_Different() => Assert.That(_complex1.Contains(_partialBad1), Is.False, "should return false when text content is different");
    [Test] public void Meta_Different_Simple() => Assert.That(_simple1.Contains(_partialBad2), Is.False, "should return false when meta is different (Single)");
    [Test] public void Meta_Different_Complex() => Assert.That(_complex1.Contains(_partialBad3), Is.False, "should return false when meta is different (Multi)");
        
    [Test] public void Match_Single_Start() => Assert.That(_complex1.Contains(_partialGood1), Is.True, "should return true when text content and meta is the same (single-start)");
    [Test] public void Match_Multi_End() => Assert.That(_complex1.Contains(_partialGood2), Is.True, "should return true when text content and meta is the same (multi-end)");
    [Test] public void Match_Multi_Mid() => Assert.That(_complex1.Contains(_partialGood3), Is.True, "should return true when text content and meta is the same (multi-middle)");

}