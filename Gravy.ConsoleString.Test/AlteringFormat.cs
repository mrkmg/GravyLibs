using System.Drawing;

namespace Gravy.ConsoleString.Test;

public class AlteringFormat
{
    private static readonly Color Red = Color.Red;
    private static readonly Color Blue = Color.Blue;

    [Test]
    public void ChangeForeground()
    {
        var cs = new ConsoleString("Test", Red);
        cs = cs.WithForeground(Blue);
        Assert.That(cs.ForegroundColors().First(), Is.EqualTo(Blue));
    }
    
    [Test]
    public void ChangeBackground()
    {
        var cs = new ConsoleString("Test", null, Red);
        cs = cs.WithBackground(Blue);
        Assert.That(cs.BackgroundColors().First(), Is.EqualTo(Blue));
    }
    
    [Test]
    public void ChangeWeight()
    {
        var cs = new ConsoleString("Test", null, null, FontWeight.Bold);
        cs = cs.WithWeight(FontWeight.Light);
        Assert.That(cs.Weights().First(), Is.EqualTo(FontWeight.Light));
    }
    
    [Test]
    public void ChangeStyle()
    {
        var cs = new ConsoleString("Test", null, null, FontWeight.Normal, FontStyle.Italic);
        cs = cs.WithStyle(FontStyle.Underline);
        Assert.That(cs.Styles().First(), Is.EqualTo(FontStyle.Italic | FontStyle.Underline));
    }
    
    [Test]
    public void ConvenienceMethods()
    {
        var cs = new ConsoleString("Test")
            .WithBackground(Red)
            .WithForeground(Blue)
            .WithBold()
            .WithUnderline()
            .WithItalic();
        var csNormal = cs.WithNormal();
        var csLight = cs.WithLight();
        var csNoUnderline = cs.WithoutUnderline();
        var csNoItalic = cs.WithoutItalic();
        var csNoForeground = cs.WithForeground(null);
        var csNoBackground = cs.WithBackground(null);
        var csNoStyle = cs.ResetStyle();
        Assert.Multiple(() =>
        {
            Assert.That(cs.BackgroundColors().First(), Is.EqualTo(Red));
            Assert.That(cs.ForegroundColors().First(), Is.EqualTo(Blue));
            Assert.That(cs.Weights().First(), Is.EqualTo(FontWeight.Bold));
            Assert.That(cs.Styles().First(), Is.EqualTo(FontStyle.Underline | FontStyle.Italic));
            
            Assert.That(csNormal.Weights().First(), Is.EqualTo(FontWeight.Normal));
            Assert.That(csLight.Weights().First(), Is.EqualTo(FontWeight.Light));
            Assert.That(csNoUnderline.Styles().First(), Is.EqualTo(FontStyle.Italic));
            Assert.That(csNoItalic.Styles().First(), Is.EqualTo(FontStyle.Underline));
            Assert.That(csNoForeground.ForegroundColors().First(), Is.EqualTo(null));
            Assert.That(csNoBackground.BackgroundColors().First(), Is.EqualTo(null));
            Assert.That(csNoStyle.Styles().First(), Is.EqualTo(FontStyle.None));
        });
    }
}