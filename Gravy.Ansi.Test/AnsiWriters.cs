using System.Drawing;

namespace Gravy.Ansi.Test;

public class AnsiWriters
{
    public TextWriter Writer { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        Writer = new StringWriter();
        Console.SetOut(Writer);
    }
    
    [TearDown]
    public void TearDown()
    {
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Writer.Dispose();
    }
    
    [Test]
    public void Bell()
    {
        Console.Out.Bell();
        Assert.That(Writer.ToString(), Is.EqualTo("\a"));
    }
    
    [Test]
    public void SetTitle()
    {
        Console.Out.SetTitle("Title");
        Assert.That(Writer.ToString(), Is.EqualTo("\e]0;Title\a"));
    }
    
    [Test]
    public void ResizeWindow()
    {
        Console.Out.ResizeWindow(10, 20);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[8;10;20t"));
    }
    
    [Test]
    public void HideCursor()
    {
        Console.Out.HideCursor();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[?25l"));
    }
    
    [Test]
    public void ShowCursor()
    {
        Console.Out.ShowCursor();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[?25h"));
    }
    
    [Test]
    public void VisualBell()
    {
        Console.Out.VisualBell();
        Assert.That(Writer.ToString(), Is.EqualTo("\eg"));
    }
    
    [Test]
    public void ResetState()
    {
        Console.Out.ResetState();
        Assert.That(Writer.ToString(), Is.EqualTo("\ec"));
    }
    
    [Test]
    public void SaveState()
    {
        Console.Out.SaveState();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[s"));
    }
    
    [Test]
    public void RestoreState()
    {
        Console.Out.RestoreState();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[u"));
    }
    
    [Test]
    public void SetCursorHome()
    {
        Console.Out.SetCursorHome();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[H"));
    }
    
    [Test]
    public void SetCursorPosition()
    {
        Console.Out.SetCursorPosition(10, 20);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10;20H"));
    }
    
    [Test]
    public void EraseScreen()
    {
        Console.Out.EraseScreen();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[2J"));
    }
    
    [Test]
    public void EraseScreenToCursor()
    {
        Console.Out.EraseScreenToCursor();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[1J"));
    }
    
    [Test]
    public void EraseScreenFromCursor()
    {
        Console.Out.EraseScreenFromCursor();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[0J"));
    }
    
    [Test]
    public void EraseLine()
    {
        Console.Out.EraseLine();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[2K"));
    }
    
    [Test]
    public void EraseLineToCursor()
    {
        Console.Out.EraseLineToCursor();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[1K"));
    }
    
    [Test]
    public void EraseLineFromCursor()
    {
        Console.Out.EraseLineFromCursor();
        Assert.That(Writer.ToString(), Is.EqualTo("\e[0K"));
    }
    
    [Test]
    public void SetModeSingle()
    {
        Console.Out.SetMode(AnsiMode.Bold, AnsiMode.Underline);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[1;4m"));
    }
    
    [Test]
    public void SetModeMultiple()
    {
        Console.Out.SetMode(new[] { AnsiMode.Bold, AnsiMode.Underline });
        Assert.That(Writer.ToString(), Is.EqualTo("\e[1;4m"));
    }
    
    [Test]
    public void SetBackgroundColorAnsi16()
    {
        Console.Out.SetBackgroundColor(Ansi16Color.Red);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[41m"));
    }
    
    [Test]
    public void SetBackgroundColorAnsi256()
    {
        Console.Out.SetBackgroundColor(Ansi256Color.Red1);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[48;5;9m"));
    }
    
    [Test]
    public void SetBackgroundColorRgb()
    {
        Console.Out.SetBackgroundColor(Color.Red);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[48;2;255;0;0m"));
    }
    
    [Test]
    public void SetForegroundColorAnsi16()
    {
        Console.Out.SetForegroundColor(Ansi16Color.Red);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[31m"));
    }
    
    [Test]
    public void SetForegroundColorAnsi256()
    {
        Console.Out.SetForegroundColor(Ansi256Color.Red1);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[38;5;9m"));
    }
    
    [Test]
    public void SetForegroundColorRgb()
    {
        Console.Out.SetForegroundColor(Color.Red);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[38;2;255;0;0m"));
    }
    
    [Test]
    public void Up()
    {
        Console.Out.Up(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10A"));
    }
    
    [Test]
    public void Down()
    {
        Console.Out.Down(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10B"));
    }
    
    [Test]
    public void Right()
    {
        Console.Out.Right(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10C"));
    }
    
    [Test]
    public void Left()
    {
        Console.Out.Left(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10D"));
    }
    
    [Test]
    public void NextLine()
    {
        Console.Out.NextLine(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10E"));
    }
    
    [Test]
    public void PreviousLine()
    {
        Console.Out.PreviousLine(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10F"));
    }
    
    [Test]
    public void Column()
    {
        Console.Out.Column(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10G"));
    }
    
    [Test]
    public void InsertLine()
    {
        Console.Out.InsertLine(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10L"));
    }
    
    [Test]
    public void DeleteLine()
    {
        Console.Out.DeleteLine(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10M"));
    }
    
    [Test]
    public void ScrollRegionUp()
    {
        Console.Out.ScrollRegionUp(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10S"));
    }
    
    [Test]
    public void ScrollRegionDown()
    {
        Console.Out.ScrollRegionDown(10);
        Assert.That(Writer.ToString(), Is.EqualTo("\e[10T"));
    }
}