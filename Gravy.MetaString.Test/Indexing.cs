namespace Gravy.MetaString.Test;

public class Indexing
{
    private readonly MetaString<int> _simple = "Hello World".Meta(0);
    private readonly MetaString<int> _complex = "Hello".Meta(1) + " " + "World".Meta(2);
        
    [Test] public void Simple_Text() => Assert.That(_simple[6].Char, Is.EqualTo('W'));
    [Test] public void Simple_Data() => Assert.That(_simple[6].Data, Is.EqualTo(0));
        
    [Test] public void Complex_Text() => Assert.That(_complex[6].Char, Is.EqualTo('W'));
    [Test] public void Complex_Data() => Assert.That(_complex[6].Data, Is.EqualTo(2));
        
}