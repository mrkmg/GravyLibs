#pragma warning disable CS8618

namespace Gravy.MetaString.Test;


public class Tests
{

    public class Constructors
    {
        public readonly MetaString<int> IntString1 = new("test");
        public readonly MetaString<int> IntString2 = new("test", 42);
        public readonly MetaString<string> StringString1 = new("test");

        [Test] public void Constructor_With_DefaultValue_Int_Text() => Assert.That(IntString1.RawText, Is.EqualTo("test"));
        [Test] public void Constructor_With_DefaultValue_Int_Value() => Assert.That(IntString1.MetaData.First().Data, Is.EqualTo(0));
        [Test] public void Constructor_With_DefaultValue_String_Text() => Assert.That(StringString1.RawText, Is.EqualTo("test"));
        [Test] public void Constructor_With_DefaultValue_String_Value() => Assert.That(StringString1.MetaData.First().Data, Is.EqualTo(null));
        [Test] public void Constructor_With_DefinedValue_Int_Text() => Assert.That(IntString2.RawText, Is.EqualTo("test"));
        [Test] public void Constructor_With_DefinedValue_Int_Value() => Assert.That(IntString2.MetaData.First().Data, Is.EqualTo(42));
    }

    public class Equality
    {
        public readonly MetaString<string> Simple1 = "Hello World".Meta("testing");
        public readonly MetaString<string> Simple2 = "Hello World".Meta("testing");
        public readonly MetaString<string> SimpleD1 = "Hello World".Meta("other");
        public readonly MetaString<string> SimpleD2 = "Hello Life".Meta("testing");
        
        public readonly MetaString<string> Complex1 = "Hello".Meta("test1") + " " + "World".Meta("test2");
        public readonly MetaString<string> Complex2 = "Hello".Meta("test1") + " " + "World".Meta("test2");
        public readonly MetaString<string> ComplexD1 = "Hello".Meta("test3") + " " + "World".Meta("test2");
        public readonly MetaString<string> ComplexD2 = "Hello".Meta("test1") + " " + "Life".Meta("test2");
        
        public readonly MetaString<TestStruct> Custom1 = "Hello World".Meta(new TestStruct { Test = 22 });
        public readonly MetaString<TestStruct> Custom2 = "Hello World".Meta(new TestStruct { Test = 27 });
        public readonly MetaString<TestStruct> Custom3 = "Hello World".Meta(new TestStruct { Test = 38 });

        [SetUp]
        public void Setup()
        {
            MetaEntry<TestStruct>.Comparer = TestStruct.TestComparer;
        }
        
        public struct TestStruct
        {
            private sealed class TestEqualityComparer : IEqualityComparer<TestStruct>
            {
                public bool Equals(TestStruct x, TestStruct y)
                {
                    return Math.Abs(x.Test - y.Test) <= 10;
                }

                public int GetHashCode(TestStruct obj)
                {
                    return obj.Test;
                }
            }

            public static IEqualityComparer<TestStruct> TestComparer { get; } = new TestEqualityComparer();

            public int Test;
        };
        
        
        [Test] public void Simple_Equal() => Assert.That(Simple1, Is.EqualTo(Simple2));
        [Test] public void Simple_Different_Meta_Not_Equal() => Assert.That(Simple1, Is.Not.EqualTo(SimpleD1));
        [Test] public void Simple_Different_Text_Not_Equal() => Assert.That(Simple1, Is.Not.EqualTo(SimpleD2));
            
        [Test] public void Complex_Equal() => Assert.That(Complex1, Is.EqualTo(Complex2));
        [Test] public void Complex_Different_Meta_Not_Equal() => Assert.That(Complex1, Is.Not.EqualTo(ComplexD1));
        [Test] public void Complex_Different_Text_Not_Equal() => Assert.That(Complex1, Is.Not.EqualTo(ComplexD2));
            
        [Test] public void Custom_Comparer_Equal() => Assert.That(Custom1, Is.EqualTo(Custom2));
        [Test] public void Custom_Compare_Not_Equal() => Assert.That(Custom1, Is.Not.EqualTo(Custom3));
    }

    public class Range
    {
        public readonly MetaString<int> Simple = "Hello World".Meta(0);
        public readonly MetaString<int> Complex = "He".Meta(1) + "ll".Meta(2) + "o " + "Wor".Meta(3) + "ld";
        
        [Test] public void Simple_Start_Offset_Text() => Assert.That(Simple[2..].RawText, Is.EqualTo("llo World"));
        [Test] public void Complex_Start_Offset_Text() => Assert.That(Complex[2..].RawText, Is.EqualTo("llo World"));
        [Test] public void Complex_Start_Offset_Meta() => Assert.That(Complex[2..].OffsetInts(), Is.EqualTo(new [] {(0, 2), (2, 0), (4, 3), (7, 0)}));
        
        [Test] public void Simple_StartEnd_Offset_Text() => Assert.That(Simple[1..8].RawText, Is.EqualTo("ello Wo"));
        [Test] public void Complex_StartEnd_Offset_Text() => Assert.That(Complex[1..8].RawText, Is.EqualTo("ello Wo"));
        [Test] public void Complex_StartEnd_Offset_Meta() => Assert.That(Complex[1..8].OffsetInts(), Is.EqualTo(new [] {(0, 1), (1, 2), (3, 0), (5, 3)}));

        [Test] public void Simple_End_Offset_Text() => Assert.That(Simple[..8].RawText, Is.EqualTo("Hello Wo"));
        [Test] public void Complex_End_Offset_Text() => Assert.That(Complex[..8].RawText, Is.EqualTo("Hello Wo"));
        [Test] public void Complex_End_Offset_Meta() => Assert.That(Complex[..8].OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 2), (4, 0), (6, 3)}));

        [Test] public void Simple_FromEnd_End_Offset_Text() => Assert.That(Simple[..^2].RawText, Is.EqualTo("Hello Wor"));
        [Test] public void Complex_FromEnd_End_Offset_Text() => Assert.That(Complex[..^2].RawText, Is.EqualTo("Hello Wor"));
        [Test] public void Complex_FromEnd_End_Offset_Meta() => Assert.That(Complex[..^2].OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 2), (4, 0), (6, 3)}));
        
        [Test] public void Simple_FromEnd_StartEnd_Offset_Text() => Assert.That(Simple[^8..^2].RawText, Is.EqualTo("lo Wor"));
        [Test] public void Complex_FromEnd_StartEnd_Offset_Text() => Assert.That(Complex[^8..^2].RawText, Is.EqualTo("lo Wor"));
        [Test] public void Complex_FromEnd_StartEnd_Offset_Meta() => Assert.That(Complex[^8..^2].OffsetInts(), Is.EqualTo(new [] {(0, 2), (1, 0), (3, 3)}));

        [Test] public void Simple_FromEnd_Outside_Range() => Assert.That(() => Simple[^20..], Throws.TypeOf<IndexOutOfRangeException>());
        [Test] public void Complex_FromEnd_Outside_Range() => Assert.That(() => Complex[^20..], Throws.TypeOf<IndexOutOfRangeException>());

        [Test] public void Simple_Outside_Range() => Assert.That(() => Simple[..20], Throws.TypeOf<IndexOutOfRangeException>());
        [Test] public void Complex_Outside_Range() => Assert.That(() => Complex[..20], Throws.TypeOf<IndexOutOfRangeException>());

        [Test] public void Simple_InvalidRange() => Assert.That(() => Simple[2..1], Throws.ArgumentException);
        [Test] public void Complex_InvalidRange() => Assert.That(() => Complex[2..1], Throws.ArgumentException);
    }

    public class Pad
    {
        public readonly MetaString<int> Simple = "Hello".Meta(0);
        public readonly MetaString<int> Complex = "Hello".Meta(1) + " " + "World".Meta(2);
        
        [Test] public void Simple_PadLeft() => Assert.That(Simple.PadLeft(10).RawText, Is.EqualTo("     Hello"));
        [Test] public void Simple_PadRight() => Assert.That(Simple.PadRight(10).RawText, Is.EqualTo("Hello     "));
        
        [Test] public void Simple_PadLeft_Noop() => Assert.That(Simple.PadLeft(5).RawText, Is.EqualTo("Hello"));
        [Test] public void Simple_PadRight_Noop() => Assert.That(Simple.PadRight(5).RawText, Is.EqualTo("Hello"));

        [Test] public void Simple_PadLeft_AltChar() => Assert.That(Simple.PadLeft('_', 10).RawText, Is.EqualTo("_____Hello"));
        [Test] public void Simple_PadRight_AltChar() => Assert.That(Simple.PadRight('_', 10).RawText, Is.EqualTo("Hello_____"));
        
        [Test] public void Complex_PadLeft_Text() => Assert.That(Complex.PadLeft(20).RawText, Is.EqualTo("         Hello World"));
        [Test] public void Complex_PadLeft_Meta() => Assert.That(Complex.PadLeft(20).OffsetInts(), Is.EqualTo(new [] { (0, 0), (9, 1), (14, 0), (15, 2) }));
        [Test] public void Complex_PadRight_Text() => Assert.That(Complex.PadRight(20).RawText, Is.EqualTo("Hello World         "));
        [Test] public void Complex_PadRight_Meta() => Assert.That(Complex.PadRight(20).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2), (11, 0) }));
        
        [Test] public void Complex_PadLeft_Noop_Text() => Assert.That(Complex.PadLeft(10).RawText, Is.EqualTo("Hello World"));
        [Test] public void Complex_PadLeft_Noop_Meta() => Assert.That(Complex.PadLeft(10).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2) }));
        [Test] public void Complex_PadRight_Noop_Text() => Assert.That(Complex.PadRight(10).RawText, Is.EqualTo("Hello World"));
        [Test] public void Complex_PadRight_Noop_Meta() => Assert.That(Complex.PadRight(10).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2) }));
        
        [Test] public void Complex_PadLeft_AltChar_Text() => Assert.That(Complex.PadLeft('_', 20).RawText, Is.EqualTo("_________Hello World"));
        [Test] public void Complex_PadLeft_AltChar_Meta() => Assert.That(Complex.PadLeft('_', 20).OffsetInts(), Is.EqualTo(new [] { (0, 0), (9, 1), (14, 0), (15, 2) }));
        [Test] public void Complex_PadRight_AltChar_Text() => Assert.That(Complex.PadRight('_', 20).RawText, Is.EqualTo("Hello World_________"));
        [Test] public void Complex_PadRight_AltChar_Meta() => Assert.That(Complex.PadRight('_', 20).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2), (11, 0) }));
    }

    public class Trim
    {
        public MetaString<int> SimpleWithWhiteSpace = "   Hello World   ".Meta(0);
        public MetaString<int> SimpleWithOneAltChar = "___Hello World___".Meta(0);
        public MetaString<int> SimpleWithMultipleAltChars = "-_-Hello World-_-".Meta(0);
        public MetaString<int> ComplexWithWhiteSpace = "   Hello".Meta(1) + " " + "World   ".Meta(2);
        public MetaString<int> ComplexWithOneAltChar = "___Hello".Meta(1) + " " + "World___".Meta(2);
        public MetaString<int> ComplexWithMultipleAltChars = "-_-Hello".Meta(1) + " " + "World-_-".Meta(2);
        public MetaString<int> ComplexWithFullyRemovedMeta = " ".Meta(1) + " ".Meta(2) +
                          "Hello World" +
                          " ".Meta(3) + " ".Meta(4);
        
        [Test] public void Empty() 
            => Assert.That(MetaString<int>.Empty.Trim().RawText, Is.EqualTo(string.Empty));
        
        [Test] public void Simple_With_White_Space() => Assert.That(SimpleWithWhiteSpace.Trim().RawText, Is.EqualTo("Hello World"));
        [Test] public void Simple_With_One_Alt_Char() => Assert.That(SimpleWithOneAltChar.Trim('_').RawText, Is.EqualTo("Hello World"));
        [Test] public void Simple_With_Multiple_Alt_Chars() => Assert.That(SimpleWithMultipleAltChars.Trim('_', '-').RawText, Is.EqualTo("Hello World"));
        [Test] public void Complex_With_White_Space() => Assert.That(ComplexWithWhiteSpace.Trim().RawText, Is.EqualTo("Hello World"));
        [Test] public void Complex_With_One_Alt_Char() => Assert.That(ComplexWithOneAltChar.Trim('_').RawText, Is.EqualTo("Hello World"));
        [Test] public void Complex_With_Multiple_Alt_Chars() => Assert.That(ComplexWithMultipleAltChars.Trim('_','-').RawText, Is.EqualTo("Hello World"));
        [Test] public void Complex_With_Fully_Removed_Meta() => Assert.That(ComplexWithFullyRemovedMeta.Trim().RawText, Is.EqualTo("Hello World"));
    }

    [Test]
    public void Substring()
    {
        var simple1 = "Hello World".Meta(0);
        var complex1 = "Hello".Meta(1) + " " + "World".Meta(2);
        Assert.Multiple(() =>
        {
            Assert.That(() => simple1.Substring(1, -1), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(simple1.Substring(3).RawText, Is.EqualTo("lo World"));
            Assert.That(complex1.Substring(3).RawText, Is.EqualTo("lo World"));
            Assert.That(complex1.Substring(3).OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 0), (3, 2)}));
            
            Assert.That(simple1.Substring(3, 6).RawText, Is.EqualTo("lo Wo"));
            Assert.That(complex1.Substring(3, 6).RawText, Is.EqualTo("lo Wo"));
            Assert.That(complex1.Substring(3, 6).OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 0), (3, 2)}));
        });
    }

    [Test]
    public void Contains()
    {
        var simple1 = "Hello World".Meta(0);
        var complex1 = "Hello".Meta(1) + " " + "World".Meta(2);
        var partialGood1 = "Hello".Meta(1);
        var partialGood2 = " " + "World".Meta(2);
        var partialGood3 = "lo".Meta(1) + " " + "Wo".Meta(2);
        var partialBad1 = "abc".Meta(1);
        var partialBad2 = "Hello".Meta(3);
        var partialBad3 = "lo".Meta(1) + " " + "Wo".Meta(3);

        Assert.Multiple(() =>
        {
            Assert.That(simple1.Contains(string.Empty), Is.True, "Empty string should be contained in any ConsoleString");
            Assert.That(complex1.Contains(MetaString<int>.Empty), Is.True, "Empty ConsoleString should be contained in any ConsoleString");
            
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
        var split1 = ("A".Meta(1) + " " + "B".Meta(2) + " " + "C".Meta(3)).Split(' ');
        var split2 = ("A".Meta(1) + "  " + "B".Meta(2) + "   " + "C".Meta(3)).Split(' ');
        var split3 = ("A".Meta(1) + "  " + "B".Meta(2) + "  " + "C".Meta(3)).Split(' ', 2);
        var split4 = ("A".Meta(1) + "  " + "B".Meta(2) + "  " + "C".Meta(3)).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var split5 = ("A".Meta(1) + "  " + "B".Meta(2) + "  " + "C".Meta(3)).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var split6 = ("A".Meta(1) + "_" + "B".Meta(2) + "_" + "C".Meta(3)).Split('_');
        var split7 = ("A".Meta(1) + "_" + "B".Meta(2) + "-" + "C".Meta(3)).Split(new [] {'_', '-'});
        var split8 = ("A".Meta(1) + "_" + "B".Meta(2) + "--" + "C".Meta(3)).Split(new [] {'_', '-'}, StringSplitOptions.RemoveEmptyEntries);
        var split9 = ("A".Meta(1) + "TEST" + "B".Meta(2) + "TEST" + "C".Meta(3)).Split("TEST");
        var split10 = ("A".Meta(1) + "TESTTEST" + "B".Meta(2) + "TESTTEST" + "C".Meta(3)).Split("TEST");
        var split11 = ("A".Meta(1) + "TEST" + "B".Meta(2) + "test" + "C".Meta(3)).Split(new [] { "TEST", "test" });

        var result1 = new [] { "A".Meta(1), "B".Meta(2), "C".Meta(3) };
        var result2 = new [] { "A".Meta(1), MetaString<int>.Empty, "B".Meta(2), MetaString<int>.Empty, MetaString<int>.Empty, "C".Meta(3) };
        var result3 = new [] { "A".Meta(1), "B".Meta(2) + "C".Meta(3) };
        var result4 = new [] { "A".Meta(1), "B".Meta(2), "C".Meta(3) };
        var result5 = new [] { "A".Meta(1), "B".Meta(2) + "C".Meta(3) };
        var result6 = new [] { "A".Meta(1), MetaString<int>.Empty, "B".Meta(2), MetaString<int>.Empty, "C".Meta(3) };

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
            Assert.That(split10, Is.EqualTo(result6));
            Assert.That(split11, Is.EqualTo(result1));
        });
    }

    [Test]
    public void Indexing()
    {
        var simple = "Hello World".Meta(0);
        var complex = "Hello".Meta(1) + " " + "World".Meta(2);

        Assert.Multiple(() =>
        {
            Assert.That(simple[0].Char, Is.EqualTo('H'));
            Assert.That(simple[6].Char, Is.EqualTo('W'));
            
            Assert.That(complex[0].Data, Is.EqualTo(1));
            Assert.That(complex[5].Data, Is.EqualTo(0));
            Assert.That(complex[6].Data, Is.EqualTo(2));
        });
    }
}