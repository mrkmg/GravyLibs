using System;

namespace Gravy.ConsoleString;

[Flags]
public enum FontStyle
{
    None = 0,
    Italic = 1,
    Underline = 2,
    Blink = 8,
    Inverse = 16,
    StrikeThrough = 32
}

public enum FontWeight
{
    Normal,
    Bold,
    Light
}