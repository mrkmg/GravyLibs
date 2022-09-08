namespace Gravy.Ansi;

public enum AnsiMode
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