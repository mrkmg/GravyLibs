using System;

namespace Gravy.ConsoleString;

[Flags]
public enum FontStyle
{
    None = 0b0,
    Italic = 0b1,
    Underline = 0b10,
    Blink = 0b1000,
    Inverse = 0b10000,
    StrikeThrough = 0b100000
}

public enum FontWeight
{
    Normal,
    Bold,
    Light
}