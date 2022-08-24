using System.Drawing;
using Gravy.MetaString;

namespace Gravy.ConsoleString.Test;

using ConsoleString = MetaString<ConsoleFormat>;

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
            Assert.That(cs.ToAnsiString(), Is.Empty);
        });
    }

    [Test]
    public void PlainString()
    {
        var cs = new ConsoleString("Test");
        Assert.That(cs.ToAnsiString(), Is.EqualTo("Test"));
    }

    [Test]
    public void ComplexString()
    { 
        var cs = new ConsoleString("Test", new(Red, Blue, FontWeight.Bold, FontStyle.Italic));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[48;2;0;0;255m\x1b[38;2;255;0;0m\x1b[1m\x1b[3mTest\x1b[0m"));
    }

    [Test]
    public void MultiChangeString()
    {
        var cs = "test".CS().FG(Blue).BG(Green).WithBold() +
                 "test".CS().FG(Red).WithUnderline();
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[48;2;0;128;0m\x1b[38;2;0;0;255m\x1b[1mtest\x1b[49m\x1b[38;2;255;0;0m\x1b[22m\x1b[4mtest\x1b[0m"));
        
    }

    [Test]
    public void ForegroundColor()
    {
        var cs = new ConsoleString("Test", new(Red));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[38;2;255;0;0mTest\x1b[0m"));
    }

    [Test]
    public void BackgroundColor()
    {
        var cs = new ConsoleString("Test", new(null, Red));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[48;2;255;0;0mTest\x1b[0m"));
    }

    [Test]
    public void WeightBold()
    {
        var cs = new ConsoleString("Test", new(null, null, FontWeight.Bold));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[1mTest\x1b[0m"));
        
    }

    [Test]
    public void WeightLight()
    {
        var cs = new ConsoleString("Test", new(null, null, FontWeight.Light));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[2mTest\x1b[0m"));
        
    }
    
    [Test]
    public void StyleItalic()
    {
        var cs = new ConsoleString("Test", new(null, null, FontWeight.Normal, FontStyle.Italic));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[3mTest\x1b[0m"));
    }
    
    [Test]
    public void StyleUnderline()
    {
        var cs = new ConsoleString("Test", new(null, null, FontWeight.Normal, FontStyle.Underline));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[4mTest\x1b[0m"));
    }
    
    [Test]
    public void StyleBlink()
    {
        var cs = new ConsoleString("Test", new(null, null, FontWeight.Normal, FontStyle.Blink));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[5mTest\x1b[0m"));
    }
    
    [Test]
    public void StyleInverse()
    {
        var cs = new ConsoleString("Test", new(null, null, FontWeight.Normal, FontStyle.Inverse));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[7mTest\x1b[0m"));
    }
    
    [Test]
    public void StyleStrikeThrough()
    {
        var cs = new ConsoleString("Test", new(null, null, FontWeight.Normal, FontStyle.StrikeThrough));
        Assert.That(cs.ToAnsiString(), Is.EqualTo("\x1b[9mTest\x1b[0m"));
    }
}