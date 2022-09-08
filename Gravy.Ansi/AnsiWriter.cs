using System.Drawing;
using JetBrains.Annotations;

namespace Gravy.Ansi;

[PublicAPI]
public static class AnsiWriter
{
    public static void Bell(this TextWriter w) => w.Write(Codes.Bell);

    public static void SetTitle(this TextWriter w, string title) => w.Write($"{Codes.Escape}]0;{title}{Codes.Bell}");

    public static void ResizeWindow(this TextWriter w, int lines, int columns) => w.Write($"{Codes.Escape}[8;{lines};{columns}t");

    public static void HideCursor(this TextWriter w) => w.Write($"{Codes.Escape}[?25l");

    public static void ShowCursor(this TextWriter w) => w.Write($"{Codes.Escape}[?25h");

    public static void VisualBell(this TextWriter w) => w.Write($"{Codes.Escape}g");

    public static void ResetState(this TextWriter w) => w.Write($"{Codes.Escape}c");

    public static void SaveState(this TextWriter w) => w.Write($"{Codes.Escape}[s");

    public static void RestoreState(this TextWriter w) => w.Write($"{Codes.Escape}[u");

    public static void SetCursorHome(this TextWriter w) => w.Write($"{Codes.Escape}[H");

    public static void SetCursorPosition(this TextWriter w, int line, int col) => w.Write($"{Codes.Escape}[{line};{col}H");

    public static void EraseScreen(this TextWriter w) => w.Write($"{Codes.Escape}[2J");

    public static void EraseScreenToCursor(this TextWriter w) => w.Write($"{Codes.Escape}[1J");

    public static void EraseScreenFromCursor(this TextWriter w) => w.Write($"{Codes.Escape}[0J");

    public static void EraseLine(this TextWriter w) => w.Write($"{Codes.Escape}[2K");

    public static void EraseLineToCursor(this TextWriter w) => w.Write($"{Codes.Escape}[1K");

    public static void EraseLineFromCursor(this TextWriter w) => w.Write($"{Codes.Escape}[0K");

    public static void SetMode(this TextWriter w, params AnsiMode[] modes) => SetMode(w, (IEnumerable<AnsiMode>)modes);
    public static void SetMode(this TextWriter w, IEnumerable<AnsiMode> modes) => w.Write($"{Codes.Escape}[{string.Join(";", modes.Select(m => (int)m))}m");
    
    private static void SetColorColor(this TextWriter w, AnsiMode mode, Color color) => w.Write($"{Codes.Escape}[{(byte)mode};2;{color.R};{color.G};{color.B}m");
    private static void Set256Color(this TextWriter w, AnsiMode mode, Ansi256Color color) => w.Write($"{Codes.Escape}[{(byte)mode};5;{(int)color}m");
    
    public static void SetBackgroundColor(this TextWriter w, AnsiColor color)
    {
        switch (color.Type)
        {
            case AnsiColorType.Ansi16:
                w.SetMode(color.Ansi16Color.BackgroundMode());
                break;
            case AnsiColorType.Ansi256:
                w.Set256Color(AnsiMode.SetBackgroundColor, color.Ansi256Color);
                break;
            case AnsiColorType.Rgb:
                w.SetColorColor(AnsiMode.SetBackgroundColor, color.RgbColor);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void SetForegroundColor(this TextWriter w, AnsiColor color)
    {
        switch (color.Type)
        {
            case AnsiColorType.Ansi16:
                w.SetMode(color.Ansi16Color.ForegroundMode());
                break;
            case AnsiColorType.Ansi256:
                w.Set256Color(AnsiMode.SetForegroundColor, color.Ansi256Color);
                break;
            case AnsiColorType.Rgb:
                w.SetColorColor(AnsiMode.SetForegroundColor, color.RgbColor);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void AnsiEscapePrefix(this TextWriter w, int count, char code) => w.Write($"{Codes.Escape}[{count}{code}");

    public static void Up(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'A');

    public static void Down(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'B');

    public static void Right(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'C');

    public static void Left(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'D');

    public static void NextLine(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'E');

    public static void PreviousLine(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'F');

    public static void Column(this TextWriter w, int col = 1) => w.AnsiEscapePrefix(col, 'G');

    public static void InsertLine(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'L');

    public static void DeleteLine(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'M');

    public static void ScrollRegionUp(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'S');

    public static void ScrollRegionDown(this TextWriter w, int count = 1) => w.AnsiEscapePrefix(count, 'T');

    private static AnsiMode ForegroundMode(this Ansi16Color color)
        => color switch
        {
            Ansi16Color.Black => AnsiMode.ForegroundBlack,
            Ansi16Color.Red => AnsiMode.ForegroundRed,
            Ansi16Color.Green => AnsiMode.ForegroundGreen,
            Ansi16Color.Yellow => AnsiMode.ForegroundYellow,
            Ansi16Color.Blue => AnsiMode.ForegroundBlue,
            Ansi16Color.Magenta => AnsiMode.ForegroundMagenta,
            Ansi16Color.Cyan => AnsiMode.ForegroundCyan,
            Ansi16Color.White => AnsiMode.ForegroundWhite,
            Ansi16Color.BrightBlack => AnsiMode.ForegroundBrightBlack,
            Ansi16Color.BrightRed => AnsiMode.ForegroundBrightRed,
            Ansi16Color.BrightGreen => AnsiMode.ForegroundBrightGreen,
            Ansi16Color.BrightYellow => AnsiMode.ForegroundBrightYellow,
            Ansi16Color.BrightBlue => AnsiMode.ForegroundBrightBlue,
            Ansi16Color.BrightMagenta => AnsiMode.ForegroundBrightMagenta,
            Ansi16Color.BrightCyan => AnsiMode.ForegroundBrightCyan,
            Ansi16Color.BrightWhite => AnsiMode.ForegroundBrightWhite,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null),
        };
    
    private static AnsiMode BackgroundMode(this Ansi16Color color)
        => color switch
        {
            Ansi16Color.Black => AnsiMode.BackgroundBlack,
            Ansi16Color.Red => AnsiMode.BackgroundRed,
            Ansi16Color.Green => AnsiMode.BackgroundGreen,
            Ansi16Color.Yellow => AnsiMode.BackgroundYellow,
            Ansi16Color.Blue => AnsiMode.BackgroundBlue,
            Ansi16Color.Magenta => AnsiMode.BackgroundMagenta,
            Ansi16Color.Cyan => AnsiMode.BackgroundCyan,
            Ansi16Color.White => AnsiMode.BackgroundWhite,
            
            Ansi16Color.BrightBlack => AnsiMode.BackgroundBrightBlack,
            Ansi16Color.BrightRed => AnsiMode.BackgroundBrightRed,
            Ansi16Color.BrightGreen => AnsiMode.BackgroundBrightGreen,
            Ansi16Color.BrightYellow => AnsiMode.BackgroundBrightYellow,
            Ansi16Color.BrightBlue => AnsiMode.BackgroundBrightBlue,
            Ansi16Color.BrightMagenta => AnsiMode.BackgroundBrightMagenta,
            Ansi16Color.BrightCyan => AnsiMode.BackgroundBrightCyan,
            Ansi16Color.BrightWhite => AnsiMode.BackgroundBrightWhite,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null),
        };

    private static class Codes
    {
        public const char Escape = '\u001B';
        public const char Bell = '\a';
    }
}