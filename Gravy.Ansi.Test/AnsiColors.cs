using System.Drawing;

namespace Gravy.Ansi.Test;

public class AnsiColors
{
    [Test]
    public void Equivalent()
    {
        var color1 = new AnsiColor(Color.Black);
        var color2 = new AnsiColor(Color.FromArgb(0xFF,0x00, 0x00, 0x00));
        var color3 = new AnsiColor(Ansi16Color.Black);
        var color4 = new AnsiColor(Ansi16Color.Black);
        Assert.Multiple(() =>
        {
            Assert.That(color1.Equivalent(color2), Is.True);
            Assert.That(color3.Equivalent(color4), Is.True);
        });
    }
    
    [Test]
    public void ImplicitConversion()
    {
        AnsiColor fromColor = Color.Black;
        AnsiColor fromAnsi16 = Ansi16Color.Black;
        AnsiColor fromAnsi256 = Ansi256Color.Black;
        
        Assert.Multiple(() =>
        {
            Assert.That(fromColor.Type, Is.EqualTo(AnsiColorType.Rgb));
            Assert.That(fromColor.RgbColor, Is.EqualTo(Color.Black));
            Assert.That(fromAnsi16.Type, Is.EqualTo(AnsiColorType.Ansi16));
            Assert.That(fromAnsi16.Ansi16Color, Is.EqualTo(Ansi16Color.Black));
            Assert.That(fromAnsi256.Type, Is.EqualTo(AnsiColorType.Ansi256));
            Assert.That(fromAnsi256.Ansi256Color, Is.EqualTo(Ansi256Color.Black));
        });
    }

    [Test]
    public void GetColor()
    {
        var color = Ansi256Color.Black.GetColor();
        Assert.That(color, Is.EqualTo(Color.FromArgb(0, 0, 0, 0)));
    }

    [Test]
    public void GetClosestAnsi256()
    {
        var color1 = Color.FromArgb(255, 255, 128, 0);
        var color2 = Color.FromArgb(255, 240, 0, 0);
        var color3 = Color.FromArgb(255, 0, 129, 255);
        var color4 = Color.FromArgb(255, 0, 0, 255);
        var color5 = Color.FromArgb(255, 128, 128, 128);
        
        Assert.Multiple(() =>
        {
            Assert.That(color1.ClosestAnsi256(), Is.EqualTo(Ansi256Color.DarkOrange1));
            Assert.That(color2.ClosestAnsi256(), Is.EqualTo(Ansi256Color.Red1));
            Assert.That(color3.ClosestAnsi256(), Is.EqualTo(Ansi256Color.DodgerBlue1));
            Assert.That(color4.ClosestAnsi256(), Is.EqualTo(Ansi256Color.Blue1));
            Assert.That(color5.ClosestAnsi256(), Is.EqualTo(Ansi256Color.Grey));
            
        });
    }
}