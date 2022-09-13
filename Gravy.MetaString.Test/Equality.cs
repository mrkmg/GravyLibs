namespace Gravy.MetaString.Test;

public class Equality
{
    private readonly MetaString<string> _simple1 = "Hello World".Meta("testing");
    private readonly MetaString<string> _simple2 = "Hello World".Meta("testing");
    private readonly MetaString<string> _simpleD1 = "Hello World".Meta("other");
    private readonly MetaString<string> _simpleD2 = "Hello Life".Meta("testing");
        
    private readonly MetaString<string> _complex1 = "Hello".Meta("test1") + " " + "World".Meta("test2");
    private readonly MetaString<string> _complex2 = "Hello".Meta("test1") + " " + "World".Meta("test2");
    private readonly MetaString<string> _complexD1 = "Hello".Meta("test3") + " " + "World".Meta("test2");
    private readonly MetaString<string> _complexD2 = "Hello".Meta("test1") + " " + "Life".Meta("test2");
        
    private readonly MetaString<TestStruct> _custom1 = "Hello World".Meta(new TestStruct { Test = 22 });
    private readonly MetaString<TestStruct> _custom2 = "Hello World".Meta(new TestStruct { Test = 27 });
    private readonly MetaString<TestStruct> _custom3 = "Hello World".Meta(new TestStruct { Test = 38 });

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
        
        
    [Test] public void Simple_Equal() => Assert.That(_simple1, Is.EqualTo(_simple2));
    [Test] public void Simple_Different_Meta_Not_Equal() => Assert.That(_simple1, Is.Not.EqualTo(_simpleD1));
    [Test] public void Simple_Different_Text_Not_Equal() => Assert.That(_simple1, Is.Not.EqualTo(_simpleD2));
            
    [Test] public void Complex_Equal() => Assert.That(_complex1, Is.EqualTo(_complex2));
    [Test] public void Complex_Different_Meta_Not_Equal() => Assert.That(_complex1, Is.Not.EqualTo(_complexD1));
    [Test] public void Complex_Different_Text_Not_Equal() => Assert.That(_complex1, Is.Not.EqualTo(_complexD2));
            
    [Test] public void Custom_Comparer_Equal() => Assert.That(_custom1, Is.EqualTo(_custom2));
    [Test] public void Custom_Compare_Not_Equal() => Assert.That(_custom1, Is.Not.EqualTo(_custom3));
}