using System.Drawing;

namespace Gravy.Ansi.Test;

public class AnsiRgbColors
{
    public AnsiColor Black { get; } = new(0, 0, 0);
    public AnsiColor Red { get; } = new(Color.Red);

    [Test]
    public void CorrectType()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Black.Type, Is.EqualTo(AnsiColorType.Rgb));
            Assert.That(Red.Type, Is.EqualTo(AnsiColorType.Rgb));
        });
    }
    
    [Test]
    public void CorrectColor()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Black.RgbColor, Is.EqualTo(Color.FromArgb(0xFF, 0, 0, 0)));
            Assert.That(Red.RgbColor, Is.EqualTo(Color.Red));
        });
    }
    
    [Test]
    public void Equal()
    {
        var black = new AnsiColor(0, 0, 0);
        Assert.Multiple(() =>
        {
            Assert.That(Black, Is.EqualTo(black));
            Assert.That(Black == black, Is.True);
        });
    }
    
    [Test]
    public void NotEqual()
    {
        var red = new AnsiColor(255, 0, 0);
        Assert.Multiple(() =>
        {
            Assert.That(Black, Is.Not.EqualTo(red));
            Assert.That(Black != red, Is.True);
        });
    }
    
    [Test]
    public void NoTransparency()
    {
        Assert.That(() => new AnsiColor(Color.FromArgb(0x99, 0, 0, 0)), Throws.ArgumentException.With.Message.EqualTo("Transparency is not supported (Parameter 'color')"));
    }
}