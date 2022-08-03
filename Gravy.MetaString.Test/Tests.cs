using System.Drawing;


namespace Gravy.MetaString.Test;


public class Tests
{
    [Test]
    public void Constructors()
    {
        var default1 = new MetaString<int>("test");
        var default2 = new MetaString<string>("test");
        var defined = new MetaString<int>("test", 42);
        
        Assert.Multiple(() =>
        {
            Assert.That(default1.MetaEntries.First().Text, Is.EqualTo("test"));
            Assert.That(default2.MetaEntries.First().Text, Is.EqualTo("test"));
            Assert.That(defined.MetaEntries.First().Text, Is.EqualTo("test"));
            Assert.That(default1.MetaEntries.First().Data, Is.EqualTo(0));
            Assert.That(default2.MetaEntries.First().Data, Is.EqualTo(null));
            Assert.That(defined.MetaEntries.First().Data, Is.EqualTo(42));
        });
    }

    struct TestStruct
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
    [Test]
    public void Equality()
    {
        var simple1 = "Hello World".Meta("testing");
        var simple2 = "Hello World".Meta("testing");
        var simpleD1 = "Hello World".Meta("other");
        var simpleD2 = "Hello Life".Meta("testing");
        
        var complex1 = "Hello".Meta("test1") + " " + "World".Meta("test2");
        var complex2 = "Hello".Meta("test1") + " " + "World".Meta("test2");
        var complexD1 = "Hello".Meta("test3") + " " + "World".Meta("test2");
        var complexD2 = "Hello".Meta("test1") + " " + "Life".Meta("test2");
        
        var custom1 = "Hello World".Meta(new TestStruct { Test = 22 });
        var custom2 = "Hello World".Meta(new TestStruct { Test = 27 });
        var custom3 = "Hello World".Meta(new TestStruct { Test = 38 });
        
        MetaEntry<TestStruct>.Comparer = TestStruct.TestComparer;

        Assert.Multiple(() =>
        {
            Assert.That(simple1, Is.EqualTo(simple2));
            Assert.That(simple1, Is.Not.EqualTo(simpleD1));
            Assert.That(simple1, Is.Not.EqualTo(simpleD2));
            
            Assert.That(complex1, Is.EqualTo(complex2));
            Assert.That(complex1, Is.Not.EqualTo(complexD1));
            Assert.That(complex1, Is.Not.EqualTo(complexD2));
            
            Assert.That(custom1, Is.EqualTo(custom2));
            Assert.That(custom1, Is.Not.EqualTo(custom3));
        });
    }
    
    [Test]
    public void Range()
    {
        var simple = "Hello World".Meta(0);
        var complex = "He".Meta(1) +
                         "ll".Meta(2) +
                         "o " +
                         "Wor".Meta(3) +
                         "ld";

        Assert.Multiple(() =>
        {
            Assert.That(simple[2..].RawText, Is.EqualTo("llo World"));
            Assert.That(complex[2..].RawText, Is.EqualTo("llo World"));
            Assert.That(complex[2..].OffsetInts(), Is.EqualTo(new [] {(0, 2), (2, 0), (4, 3), (7, 0)}));
            
            Assert.That(simple[1..8].RawText, Is.EqualTo("ello Wo"));
            Assert.That(complex[1..8].RawText, Is.EqualTo("ello Wo"));
            Assert.That(complex[1..8].OffsetInts(), Is.EqualTo(new [] {(0, 1), (1, 2), (3, 0), (5, 3)}));
            
            Assert.That(simple[..8].RawText, Is.EqualTo("Hello Wo"));
            Assert.That(complex[..8].RawText, Is.EqualTo("Hello Wo"));
            Assert.That(complex[..8].OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 2), (4, 0), (6, 3)}));
            
            Assert.That(simple[..^2].RawText, Is.EqualTo("Hello Wor"));
            Assert.That(complex[..^2].RawText, Is.EqualTo("Hello Wor"));
            Assert.That(complex[..^2].OffsetInts(), Is.EqualTo(new [] {(0, 1), (2, 2), (4, 0), (6, 3)}));
            
            Assert.That(simple[^8..^2].RawText, Is.EqualTo("lo Wor"));
            Assert.That(complex[^8..^2].RawText, Is.EqualTo("lo Wor"));
            Assert.That(complex[^8..^2].OffsetInts(), Is.EqualTo(new [] {(0, 2), (1, 0), (3, 3)}));
            
            Assert.That(() => simple[^20..], Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(() => complex[^20..], Throws.TypeOf<IndexOutOfRangeException>());
            
            Assert.That(() => simple[..20], Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(() => complex[..20], Throws.TypeOf<IndexOutOfRangeException>());
            
            Assert.That(() => simple[2..1], Throws.ArgumentException);
            Assert.That(() => simple[2..1], Throws.ArgumentException);

        });
    }

    [Test]
    public void Pad()
    {
        var cs = "Hello".Meta(0);
        var cs2 = "Hello".Meta(1) + " " + "World".Meta(2);
        
        Assert.Multiple(() =>
        {
            Assert.That(cs.PadLeft(5).RawText, Is.EqualTo("Hello"));
            Assert.That(cs.PadRight(5).RawText, Is.EqualTo("Hello"));
            
            Assert.That(cs.PadLeft(10).RawText, Is.EqualTo("     Hello"));
            Assert.That(cs.PadRight(10).RawText, Is.EqualTo("Hello     "));
            
            Assert.That(cs.PadLeft('_', 10).RawText, Is.EqualTo("_____Hello"));
            Assert.That(cs.PadRight('_', 10).RawText, Is.EqualTo("Hello_____"));
            
            Assert.That(cs2.PadLeft(10).RawText, Is.EqualTo("Hello World"));
            Assert.That(cs2.PadLeft(10).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2) }));
            Assert.That(cs2.PadRight(10).RawText, Is.EqualTo("Hello World"));
            Assert.That(cs2.PadRight(10).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2) }));
            
            Assert.That(cs2.PadLeft(20).RawText, Is.EqualTo("         Hello World"));
            Assert.That(cs2.PadLeft(20).OffsetInts(), Is.EqualTo(new [] { (0, 0), (9, 1), (14, 0), (15, 2) }));
            Assert.That(cs2.PadRight(20).RawText, Is.EqualTo("Hello World         "));
            Assert.That(cs2.PadRight(20).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2), (11, 0) }));
            
            Assert.That(cs2.PadLeft('_', 20).RawText, Is.EqualTo("_________Hello World"));
            Assert.That(cs2.PadLeft('_', 20).OffsetInts(), Is.EqualTo(new [] { (0, 0), (9, 1), (14, 0), (15, 2) }));
            Assert.That(cs2.PadRight('_', 20).RawText, Is.EqualTo("Hello World_________"));
            Assert.That(cs2.PadRight('_', 20).OffsetInts(), Is.EqualTo(new [] { (0, 1), (5, 0), (6, 2), (11, 0) }));
        });
    }

    [Test]
    public void Trim()
    {
        var simple1 = "   Hello World   ".Meta(0);
        var simple2 = "___Hello World___".Meta(0);
        var simple3 = "-_-Hello World-_-".Meta(0);
        var complex1 = "   Hello".Meta(1) + " " + "World   ".Meta(2);
        var complex2 = "___Hello".Meta(1) + " " + "World___".Meta(2);
        var complex3 = "-_-Hello".Meta(1) + " " + "World-_-".Meta(2);
        var fullRemove1 = " ".Meta(1) + " ".Meta(2) +
                          "Hello World" +
                          " ".Meta(3) + " ".Meta(4);
        
        Assert.Multiple(() =>
        {
            Assert.That(MetaString<int>.Empty.Trim().RawText, Is.EqualTo(string.Empty));
            
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