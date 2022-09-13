using Gravy.Ansi;

namespace Gravy.ConsoleString.Test;

public class ConsoleFormatTest
{
    private static readonly ConsoleFormat Empty = new();
    private static readonly AnsiColor Red = Ansi16Color.Red;
    private static readonly AnsiColor Green = Ansi16Color.Green;
    private const FontStyle AllFormats = FontStyle.Italic | FontStyle.Underline | FontStyle.StrikeThrough | FontStyle.Inverse | FontStyle.Blink;

    // With Helpers
    [Test] public void WithForeground() => Assert.That(Empty.WithForeground(Red).ForegroundColor, Is.EqualTo(Red));
    [Test] public void WithBackground() => Assert.That(Empty.WithBackground(Green).BackgroundColor, Is.EqualTo(Green));
    [Test] public void WithNormal() => Assert.That(new ConsoleFormat(FontWeight.Bold).WithNormal().Weight, Is.EqualTo(FontWeight.Normal));
    [Test] public void WithBold() => Assert.That(Empty.WithBold().Weight, Is.EqualTo(FontWeight.Bold));
    [Test] public void WithLight() => Assert.That(Empty.WithLight().Weight, Is.EqualTo(FontWeight.Light));
    [Test] public void WithItalic() => Assert.That(Empty.WithItalic().Styles, Is.EqualTo(FontStyle.Italic));
    [Test] public void WithUnderline() => Assert.That(Empty.WithUnderline().Styles, Is.EqualTo(FontStyle.Underline));
    [Test] public void WithBlink() => Assert.That(Empty.WithBlink().Styles, Is.EqualTo(FontStyle.Blink));
    [Test] public void WithInverse() => Assert.That(Empty.WithInverse().Styles, Is.EqualTo(FontStyle.Inverse));
    [Test] public void WithStrikeThrough() => Assert.That(Empty.WithStrikeThrough().Styles, Is.EqualTo(FontStyle.StrikeThrough));
    
    // Without Helpers
    [Test] public void WithoutItalic() => Assert.That(new ConsoleFormat(AllFormats).WithoutItalic().Styles, Is.EqualTo(AllFormats & ~FontStyle.Italic));
    [Test] public void WithoutUnderline() => Assert.That(new ConsoleFormat(AllFormats).WithoutUnderline().Styles, Is.EqualTo(AllFormats & ~FontStyle.Underline));
    [Test] public void WithoutBlink() => Assert.That(new ConsoleFormat(AllFormats).WithoutBlink().Styles, Is.EqualTo(AllFormats & ~FontStyle.Blink));
    [Test] public void WithoutInverse() => Assert.That(new ConsoleFormat(AllFormats).WithoutInverse().Styles, Is.EqualTo(AllFormats & ~FontStyle.Inverse));
    [Test] public void WithoutStrikeThrough() => Assert.That(new ConsoleFormat(AllFormats).WithoutStrikeThrough().Styles, Is.EqualTo(AllFormats & ~FontStyle.StrikeThrough));
    
}