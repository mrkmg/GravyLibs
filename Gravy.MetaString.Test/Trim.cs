namespace Gravy.MetaString.Test;

public class Trim
{
    private readonly MetaString<int> _simpleWithWhiteSpace = "   Hello World   ".Meta(0);
    private readonly MetaString<int> _simpleWithOneAltChar = "___Hello World___".Meta(0);
    private readonly MetaString<int> _simpleWithMultipleAltChars = "-_-Hello World-_-".Meta(0);
    private readonly MetaString<int> _complexWithWhiteSpace = "   Hello".Meta(1) + " " + "World   ".Meta(2);
    private readonly MetaString<int> _complexWithOneAltChar = "___Hello".Meta(1) + " " + "World___".Meta(2);
    private readonly MetaString<int> _complexWithMultipleAltChars = "-_-Hello".Meta(1) + " " + "World-_-".Meta(2);
    private readonly MetaString<int> _complexWithFullyRemovedMeta = " ".Meta(1) + " ".Meta(2) +
                                                                    "Hello World" +
                                                                    " ".Meta(3) + " ".Meta(4);
        
    [Test] public void Empty() 
        => Assert.That(MetaString<int>.Empty.Trim().RawText, Is.EqualTo(string.Empty));
        
    [Test] public void Simple_With_White_Space() => Assert.That(_simpleWithWhiteSpace.Trim().RawText, Is.EqualTo("Hello World"));
    [Test] public void Simple_With_One_Alt_Char() => Assert.That(_simpleWithOneAltChar.Trim('_').RawText, Is.EqualTo("Hello World"));
    [Test] public void Simple_With_Multiple_Alt_Chars() => Assert.That(_simpleWithMultipleAltChars.Trim('_', '-').RawText, Is.EqualTo("Hello World"));
    [Test] public void Complex_With_White_Space() => Assert.That(_complexWithWhiteSpace.Trim().RawText, Is.EqualTo("Hello World"));
    [Test] public void Complex_With_One_Alt_Char() => Assert.That(_complexWithOneAltChar.Trim('_').RawText, Is.EqualTo("Hello World"));
    [Test] public void Complex_With_Multiple_Alt_Chars() => Assert.That(_complexWithMultipleAltChars.Trim('_','-').RawText, Is.EqualTo("Hello World"));
    [Test] public void Complex_With_Fully_Removed_Meta() => Assert.That(_complexWithFullyRemovedMeta.Trim().RawText, Is.EqualTo("Hello World"));
}