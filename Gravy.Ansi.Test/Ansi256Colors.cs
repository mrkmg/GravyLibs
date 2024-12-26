namespace Gravy.Ansi.Test;

public class Ansi256Colors
{
    public AnsiColor Black { get; } = new(Ansi256Color.Black);
    public AnsiColor Red { get; } = new(Ansi256Color.Red1);
    
    [Test]
    public void CorrectType()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Black.Type, Is.EqualTo(AnsiColorType.Ansi256));
            Assert.That(Red.Type, Is.EqualTo(AnsiColorType.Ansi256));
        });
    }
    
    [Test]
    public void CorrectColor()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Black.Ansi256Color, Is.EqualTo(Ansi256Color.Black));
            Assert.That(Red.Ansi256Color, Is.EqualTo(Ansi256Color.Red1));
        });
    }
    
    [Test]
    public void Equal()
    {
        var black = new AnsiColor(Ansi256Color.Black);
        Assert.Multiple(() =>
        {
            Assert.That(Black, Is.EqualTo(black));
            Assert.That(Black == black, Is.True);
        });
    }
    
    [Test]
    public void NotEqual()
    {
        var red = new AnsiColor(Ansi256Color.Red1);
        Assert.Multiple(() =>
        {
            Assert.That(Black, Is.Not.EqualTo(red));
            Assert.That(Black != red, Is.True);
        });
    }
    
    [Test]
    public void FromInt()
    {
        var red = new AnsiColor(9);
        Assert.Multiple(() =>
        {
            Assert.That(red.Type, Is.EqualTo(AnsiColorType.Ansi256));
            Assert.That(red.Ansi256Color, Is.EqualTo(Ansi256Color.Red1));
            Assert.That(() => new AnsiColor(-1), Throws.InstanceOf<ArgumentOutOfRangeException>().With.Message.EqualTo("Must be between 0 and 255 (Parameter 'ansi256Color')"+Environment.NewLine+"Actual value was -1."));
            Assert.That(() => new AnsiColor(256), Throws.InstanceOf<ArgumentOutOfRangeException>().With.Message.EqualTo("Must be between 0 and 255 (Parameter 'ansi256Color')"+Environment.NewLine+"Actual value was 256."));
        });
    }
}