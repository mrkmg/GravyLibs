using System.Drawing;

namespace Gravy.ConsoleString.Test;

public class SimpleUsage
{
    private static readonly Color Red = Color.Red;
    private static readonly Color Blue = Color.Blue;
    private static readonly Color Green = Color.Green;

    [Test]
    public void Empty()
    {
        var cs = ConsoleString.Empty;
        Assert.Multiple(() =>
        {
            Assert.That(cs.ToString(), Is.Empty);
        });
    }

    [Test]
    public void PlainString()
    {
        var cs = new ConsoleString("Test");
        Assert.That(cs.ToString(), Is.EqualTo("Test"));
    }

    [Test]
    public void ComplexString()
    { 
        var cs = new ConsoleString("Test", Red, Blue, FontStyle.Bold | FontStyle.Italic);
        Assert.That(cs.ToString(), Is.EqualTo("\x1b[48;2;0;0;255m\x1b[38;2;255;0;0m\x1b[1m\x1b[3mTest\x1b[0m"));
    }

    [Test]
    public void MultiChangeString()
    {
        var cs = "test".CS().FG(Blue).BG(Green).WithBold() +
                 "test".CS().FG(Red).WithUnderline();
        Assert.That(cs.ToString(), Is.EqualTo("\x1b[48;2;0;128;0m\x1b[38;2;0;0;255m\x1b[1mtest\x1b[49m\x1b[38;2;255;0;0m\x1b[22m\x1b[4mtest\x1b[0m"));
        
    }

    [Test]
    public void ForegroundColor()
    {
        var cs = new ConsoleString("Test", Red);
        Assert.That(cs.ToString(), Is.EqualTo("\x1b[38;2;255;0;0mTest\x1b[0m"));
    }

    [Test]
    public void BackgroundColor()
    {
        var cs = new ConsoleString("Test", null, Red);
        Assert.That(cs.ToString(), Is.EqualTo("\x1b[48;2;255;0;0mTest\x1b[0m"));
    }

    [Test]
    public void StyleBold()
    {
        var cs = new ConsoleString("Test", null, null, FontStyle.Bold);
        Assert.That(cs.ToString(), Is.EqualTo("\x1b[1mTest\x1b[0m"));
        
    }
    
    [Test]
    public void StyleItalic()
    {
        var cs = new ConsoleString("Test", null, null, FontStyle.Italic);
        Assert.That(cs.ToString(), Is.EqualTo("\x1b[3mTest\x1b[0m"));
    }
    
    [Test]
    public void StyleUnderline()
    {
        var cs = new ConsoleString("Test", null, null, FontStyle.Underline);
        Assert.That(cs.ToString(), Is.EqualTo("\x1b[4mTest\x1b[0m"));
    }
}