namespace Gravy.Ansi.Test;

public class Ansi16Colors
{
    public AnsiColor Black { get; } = new(Ansi16Color.Black);
    public AnsiColor Red { get; } = new(Ansi16Color.Red);
    
    [Test]
    public void CorrectType()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Black.Type, Is.EqualTo(AnsiColorType.Ansi16));
            Assert.That(Red.Type, Is.EqualTo(AnsiColorType.Ansi16));
        });
    }
    
    [Test]
    public void CorrectColor()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Black.Ansi16Color, Is.EqualTo(Ansi16Color.Black));
            Assert.That(Red.Ansi16Color, Is.EqualTo(Ansi16Color.Red));
        });
    }
    
    [Test]
    public void Equal()
    {
        var black = new AnsiColor(Ansi16Color.Black);
        Assert.Multiple(() =>
        {
            Assert.That(Black, Is.EqualTo(black));
            Assert.That(Black == black, Is.True);
        });
    }
    
    [Test]
    public void NotEqual()
    {
        var red = new AnsiColor(Ansi16Color.Red);
        Assert.Multiple(() =>
        {
            Assert.That(Black, Is.Not.EqualTo(red));
            Assert.That(Black != red, Is.True);
        });
    }
}