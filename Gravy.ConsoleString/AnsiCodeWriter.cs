using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Gravy.ConsoleString;

[PublicAPI]
public static class AnsiCodeWriter
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

    public static void SetMode(this TextWriter w, params Mode[] modes) => SetMode(w, (IEnumerable<Mode>)modes);
    public static void SetMode(this TextWriter w, IEnumerable<Mode> modes) => w.Write($"{Codes.Escape}[{string.Join(";", modes.AsEnumerable().Select(m => (int)m))}m");

    private static void SetColorColor(this TextWriter w, Mode mode, Color color) => w.Write($"{Codes.Escape}[{(byte)mode};2;{color.R};{color.G};{color.B}m");

    public static void SetBackgroundColor(this TextWriter w, AnsiColor color)
    {
        if (color.IsThemeColor) w.SetMode(color.ThemeColor.BackgroundMode());
        else w.SetColorColor(Mode.SetBackgroundColor, color.SystemColor);
    }

    public static void SetForegroundColor(this TextWriter w, AnsiColor color)
    {
        if (color.IsThemeColor) w.SetMode(color.ThemeColor.ForegroundMode());
        else w.SetColorColor(Mode.SetForegroundColor, color.SystemColor);
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

    private static Mode ForegroundMode(this ConsoleThemeColor color)
        => color switch
        {
            ConsoleThemeColor.Black => Mode.ForegroundBlack,
            ConsoleThemeColor.Red => Mode.ForegroundRed,
            ConsoleThemeColor.Green => Mode.ForegroundGreen,
            ConsoleThemeColor.Yellow => Mode.ForegroundYellow,
            ConsoleThemeColor.Blue => Mode.ForegroundBlue,
            ConsoleThemeColor.Magenta => Mode.ForegroundMagenta,
            ConsoleThemeColor.Cyan => Mode.ForegroundCyan,
            ConsoleThemeColor.White => Mode.ForegroundWhite,
            ConsoleThemeColor.BrightBlack => Mode.ForegroundBrightBlack,
            ConsoleThemeColor.BrightRed => Mode.ForegroundBrightRed,
            ConsoleThemeColor.BrightGreen => Mode.ForegroundBrightGreen,
            ConsoleThemeColor.BrightYellow => Mode.ForegroundBrightYellow,
            ConsoleThemeColor.BrightBlue => Mode.ForegroundBrightBlue,
            ConsoleThemeColor.BrightMagenta => Mode.ForegroundBrightMagenta,
            ConsoleThemeColor.BrightCyan => Mode.ForegroundBrightCyan,
            ConsoleThemeColor.BrightWhite => Mode.ForegroundBrightWhite,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    
    private static Mode BackgroundMode(this ConsoleThemeColor color)
        => color switch
        {
            ConsoleThemeColor.Black => Mode.BackgroundBlack,
            ConsoleThemeColor.Red => Mode.BackgroundRed,
            ConsoleThemeColor.Green => Mode.BackgroundGreen,
            ConsoleThemeColor.Yellow => Mode.BackgroundYellow,
            ConsoleThemeColor.Blue => Mode.BackgroundBlue,
            ConsoleThemeColor.Magenta => Mode.BackgroundMagenta,
            ConsoleThemeColor.Cyan => Mode.BackgroundCyan,
            ConsoleThemeColor.White => Mode.BackgroundWhite,
            
            ConsoleThemeColor.BrightBlack => Mode.BackgroundBrightBlack,
            ConsoleThemeColor.BrightRed => Mode.BackgroundBrightRed,
            ConsoleThemeColor.BrightGreen => Mode.BackgroundBrightGreen,
            ConsoleThemeColor.BrightYellow => Mode.BackgroundBrightYellow,
            ConsoleThemeColor.BrightBlue => Mode.BackgroundBrightBlue,
            ConsoleThemeColor.BrightMagenta => Mode.BackgroundBrightMagenta,
            ConsoleThemeColor.BrightCyan => Mode.BackgroundBrightCyan,
            ConsoleThemeColor.BrightWhite => Mode.BackgroundBrightWhite,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };

    private static class Codes
    {
        public const char Escape = '\u001B';
        public const char Bell = '\a';
    }
}

public enum Mode
{
    Reset = 0,
    Bold = 1,
    Faint = 2,
    Italic = 3,
    Underline = 4,
    Blink = 5,
    Inverse = 7,
    StrikeThrough = 9,
    Normal = 0x16, // 0x00000016
    NoItalic = 0x17, // 0x00000017
    NoUnderline = 24, // 0x00000018
    NoBlink = 25, // 0x00000019
    NoInverse = 27, // 0x0000001B
    NoStrikeThrough = 29, // 0x0000001D
    ForegroundBlack = 30, // 0x0000001E
    ForegroundRed = 31, // 0x0000001F
    ForegroundGreen = 32, // 0x00000020
    ForegroundYellow = 33, // 0x00000021
    ForegroundBlue = 34, // 0x00000022
    ForegroundMagenta = 35, // 0x00000023
    ForegroundCyan = 36, // 0x00000024
    ForegroundWhite = 37, // 0x00000025
    SetForegroundColor = 38, // 0x00000026
    ForegroundDefault = 39, // 0x00000027
    BackgroundBlack = 40, // 0x00000028
    BackgroundRed = 41, // 0x00000029
    BackgroundGreen = 42, // 0x0000002A
    BackgroundYellow = 43, // 0x0000002B
    BackgroundBlue = 44, // 0x0000002C
    BackgroundMagenta = 45, // 0x0000002D
    BackgroundCyan = 46, // 0x0000002E
    BackgroundWhite = 47, // 0x0000002F
    SetBackgroundColor = 48, // 0x00000030
    BackgroundDefault = 49, // 0x00000031
    ForegroundBrightBlack = 90, // 0x0000005A
    ForegroundBrightRed = 91, // 0x0000005B
    ForegroundBrightGreen = 92, // 0x0000005C
    ForegroundBrightYellow = 93, // 0x0000005D
    ForegroundBrightBlue = 94, // 0x0000005E
    ForegroundBrightMagenta = 95, // 0x0000005F
    ForegroundBrightCyan = 96, // 0x00000060
    ForegroundBrightWhite = 97, // 0x00000061
    BackgroundBrightBlack = 100, // 0x00000064
    BackgroundBrightRed = 101, // 0x00000065
    BackgroundBrightGreen = 102, // 0x00000066
    BackgroundBrightYellow = 103, // 0x00000067
    BackgroundBrightBlue = 104, // 0x00000068
    BackgroundBrightMagenta = 105, // 0x00000069
    BackgroundBrightCyan = 106, // 0x0000006A
    BackgroundBrightWhite = 107, // 0x0000006B
}

public enum ConsoleThemeColor
{
    Black,
    Red,
    Green,
    Yellow,
    Blue,
    Magenta,
    Cyan,
    White,
    BrightBlack,
    BrightRed,
    BrightGreen,
    BrightYellow,
    BrightBlue,
    BrightMagenta,
    BrightCyan,
    BrightWhite
}

public readonly struct AnsiColor
{
    public readonly bool IsThemeColor;
    public readonly ConsoleThemeColor ThemeColor;
    public readonly Color SystemColor;

    public AnsiColor(ConsoleThemeColor color)
    {
        SystemColor = default;
        IsThemeColor = true;
        ThemeColor = color;
    }

    public AnsiColor(Color color)
    {
        if (color.A != 0xFF)
            throw new ArgumentException("Transparency is not supported", nameof(color));
        SystemColor = color;
        IsThemeColor = false;
        ThemeColor = default;
    }

    public AnsiColor(byte r, byte g, byte b) : this(Color.FromArgb(0xFF, r, g, b)) { }

    public bool Equals(AnsiColor other) 
        => (IsThemeColor && other.IsThemeColor && ThemeColor == other.ThemeColor) || 
           (!IsThemeColor && !other.IsThemeColor && SystemColor.IsEquivalent(other.SystemColor));
    public override bool Equals(object? obj) => obj is AnsiColor other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(IsThemeColor, ThemeColor, SystemColor);
    public static bool operator ==(AnsiColor left, AnsiColor right) => left.Equals(right);
    public static bool operator !=(AnsiColor left, AnsiColor right) => !left.Equals(right);
    
    public static implicit operator AnsiColor(ConsoleThemeColor color) => new(color);
    public static implicit operator AnsiColor(Color color) => new(color);
}

public static class WindowsConsole
{
    public static bool TryEnableVirtualTerminalProcessing()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return false;
        try
        {
            var stdHandle = NativeMethods.GetStdHandle(-11);
            NativeMethods.GetConsoleMode(stdHandle, out var mode);
            NativeMethods.SetConsoleMode(stdHandle, mode | 4);
            NativeMethods.GetConsoleMode(stdHandle, out mode);
            return (mode & 4) == 4;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
    }

    private static class NativeMethods
    {
        private const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [DllImport(Kernel32, SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport(Kernel32, SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int handle);
    }
}