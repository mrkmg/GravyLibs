#pragma warning disable CS8618

namespace Gravy.MetaString.Test;

public class Constructors
{
    private readonly MetaString<int> _intString1 = new("test");
    private readonly MetaString<int> _intString2 = new("test", 42);
    private readonly MetaString<string> _stringString1 = new("test");

    [Test] public void Constructor_With_DefaultValue_Int_Text() => Assert.That(_intString1.RawText, Is.EqualTo("test"));
    [Test] public void Constructor_With_DefaultValue_Int_Value() => Assert.That(_intString1.MetaData.First().Data, Is.EqualTo(0));
    [Test] public void Constructor_With_DefaultValue_String_Text() => Assert.That(_stringString1.RawText, Is.EqualTo("test"));
    [Test] public void Constructor_With_DefaultValue_String_Value() => Assert.That(_stringString1.MetaData.First().Data, Is.EqualTo(null));
    [Test] public void Constructor_With_DefinedValue_Int_Text() => Assert.That(_intString2.RawText, Is.EqualTo("test"));
    [Test] public void Constructor_With_DefinedValue_Int_Value() => Assert.That(_intString2.MetaData.First().Data, Is.EqualTo(42));
}