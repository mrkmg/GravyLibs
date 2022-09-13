namespace Gravy.MetaString.Test;

public class Split
{
    private readonly MetaString<int> _source1 = "A".Meta(1) + " " + "B".Meta(2) + " " + "C".Meta(3);
    private readonly MetaString<int> _source2 = "A".Meta(1) + "  " + "B".Meta(2) + "   " + "C".Meta(3);
    private readonly MetaString<int> _source3 = "A".Meta(1) + "  " + "B".Meta(2) + "  " + "C".Meta(3);
    private readonly MetaString<int> _source4 = "A".Meta(1) + "  " + "B".Meta(2) + "  " + "C".Meta(3);
    private readonly MetaString<int> _source5 = "A".Meta(1) + "  " + "B".Meta(2) + "  " + "C".Meta(3);
    private readonly MetaString<int> _source6 = "A".Meta(1) + "_" + "B".Meta(2) + "_" + "C".Meta(3);
    private readonly MetaString<int> _source7 = "A".Meta(1) + "_" + "B".Meta(2) + "-" + "C".Meta(3);
    private readonly MetaString<int> _source8 = "A".Meta(1) + "_" + "B".Meta(2) + "--" + "C".Meta(3);
    private readonly MetaString<int> _source9 = "A".Meta(1) + "TEST" + "B".Meta(2) + "TEST" + "C".Meta(3);
    private readonly MetaString<int> _source10 = "A".Meta(1) + "TESTTEST" + "B".Meta(2) + "TESTTEST" + "C".Meta(3);
    private readonly MetaString<int> _source11 = "A".Meta(1) + "TEST" + "B".Meta(2) + "test" + "C".Meta(3);

    private readonly MetaString<int>[] _result1 = { "A".Meta(1), "B".Meta(2), "C".Meta(3) };
    private readonly MetaString<int>[] _result2 = { "A".Meta(1), MetaString<int>.Empty, "B".Meta(2), MetaString<int>.Empty, MetaString<int>.Empty, "C".Meta(3) };
    private readonly MetaString<int>[] _result3 = { "A".Meta(1), "B".Meta(2) + "C".Meta(3) };
    private readonly MetaString<int>[] _result4 = { "A".Meta(1), "B".Meta(2), "C".Meta(3) };
    private readonly MetaString<int>[] _result5 = { "A".Meta(1), "B".Meta(2) + "C".Meta(3) };
    private readonly MetaString<int>[] _result6 = { "A".Meta(1), MetaString<int>.Empty, "B".Meta(2), MetaString<int>.Empty, "C".Meta(3) };

    [Test] public void Single_Char() => Assert.That(_source1.Split(' '), Is.EqualTo(_result1));
    [Test] public void Single_Char_With_Empties() => Assert.That(_source2.Split(' '), Is.EqualTo(_result2));
    [Test] public void Single_Char_Limit() => Assert.That(_source3.Split(' ', 2), Is.EqualTo(_result3));
    [Test] public void Single_Char_Remove_Empties() => Assert.That(_source4.Split(' ', StringSplitOptions.RemoveEmptyEntries), Is.EqualTo(_result4));
    [Test] public void Single_Char_Remove_Empties_Limit() => Assert.That(_source5.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries), Is.EqualTo(_result5));
    [Test] public void Single_Char_Alt() => Assert.That(_source6.Split('_'), Is.EqualTo(_result1));
    [Test] public void Multiple_Chars() => Assert.That(_source7.Split(new [] {'_', '-'}), Is.EqualTo(_result1));
    [Test] public void Multiple_Chars_Remove_Empties() => Assert.That(_source8.Split(new [] {'_', '-'}, StringSplitOptions.RemoveEmptyEntries), Is.EqualTo(_result1));
    [Test] public void Single_String() => Assert.That(_source9.Split("TEST"), Is.EqualTo(_result1));
    [Test] public void Single_String_With_Empties() => Assert.That(_source10.Split("TEST"), Is.EqualTo(_result6));
    [Test] public void Multiple_Strings() => Assert.That(_source11.Split(new [] { "TEST", "test" }), Is.EqualTo(_result1));
}