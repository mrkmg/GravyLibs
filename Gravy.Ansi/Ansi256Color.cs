using System.Drawing;

namespace Gravy.Ansi;

public enum Ansi256Color
{
    Aqua = 14, // #00ffff
    Aquamarine1 = 122, // #87ffd7
    Aquamarine2 = 86, // #5fffd7
    Aquamarine3 = 79, // #5fd7af
    Black = 0, // #000000
    Blue1 = 12, // #0000ff
    Blue2 = 21, // #0000ff
    Blue3 = 19, // #0000af
    Blue4 = 20, // #0000d7
    BlueViolet = 57, // #5f00ff
    CadetBlue1 = 72, // #5faf87
    CadetBlue2 = 73, // #5fafaf
    Chartreuse1 = 118, // #87ff00
    Chartreuse2 = 112, // #87d700
    Chartreuse3 = 82, // #5fff00
    Chartreuse4 = 70, // #5faf00
    Chartreuse5 = 76, // #5fd700
    Chartreuse6 = 64, // #5f8700
    CornflowerBlue = 69, // #5f87ff
    Cornsilk1 = 230, // #ffffd7
    Cyan1 = 51, // #00ffff
    Cyan2 = 50, // #00ffd7
    Cyan3 = 43, // #00d7af
    DarkBlue = 18, // #000087
    DarkCyan = 36, // #00af87
    DarkGoldenrod = 136, // #af8700
    DarkGreen = 22, // #005f00
    DarkKhaki = 143, // #afaf5f
    DarkMagenta1 = 90, // #870087
    DarkMagenta2 = 91, // #8700af
    DarkOliveGreen1 = 191, // #d7ff5f
    DarkOliveGreen2 = 192, // #d7ff87
    DarkOliveGreen3 = 155, // #afff5f
    DarkOliveGreen4 = 107, // #87af5f
    DarkOliveGreen5 = 113, // #87d75f
    DarkOliveGreen6 = 149, // #afd75f
    DarkOrange1 = 208, // #ff8700
    DarkOrange2 = 130, // #af5f00
    DarkOrange3 = 166, // #d75f00
    DarkRed1 = 52, // #5f0000
    DarkRed2 = 88, // #870000
    DarkSeaGreen1 = 108, // #87af87
    DarkSeaGreen2 = 158, // #afffd7
    DarkSeaGreen3 = 193, // #d7ffaf
    DarkSeaGreen4 = 151, // #afd7af
    DarkSeaGreen5 = 157, // #afffaf
    DarkSeaGreen6 = 115, // #87d7af
    DarkSeaGreen7 = 150, // #afd787
    DarkSeaGreen8 = 65, // #5f875f
    DarkSeaGreen9 = 71, // #5faf5f
    DarkSlateGray1 = 123, // #87ffff
    DarkSlateGray2 = 87, // #5fffff
    DarkSlateGray3 = 116, // #87d7d7
    DarkTurquoise = 44, // #00d7d7
    DarkViolet1 = 128, // #af00d7
    DarkViolet2 = 92, // #8700d7
    DeepPink1 = 198, // #ff0087
    DeepPink2 = 199, // #ff00af
    DeepPink3 = 197, // #ff005f
    DeepPink4 = 161, // #d7005f
    DeepPink5 = 162, // #d70087
    DeepPink6 = 125, // #af005f
    DeepPink7 = 53, // #5f005f
    DeepPink8 = 89, // #87005f
    DeepSkyBlue1 = 39, // #00afff
    DeepSkyBlue2 = 38, // #00afd7
    DeepSkyBlue3 = 31, // #0087af
    DeepSkyBlue4 = 32, // #0087d7
    DeepSkyBlue5 = 23, // #005f5f
    DeepSkyBlue6 = 24, // #005f87
    DeepSkyBlue7 = 25, // #005faf
    DodgerBlue1 = 33, // #0087ff
    DodgerBlue2 = 27, // #005fff
    DodgerBlue3 = 26, // #005fd7
    Fuchsia = 13, // #ff00ff
    Gold1 = 220, // #ffd700
    Gold2 = 142, // #afaf00
    Gold3 = 178, // #d7af00
    Green1 = 2, // #008000
    Green2 = 46, // #00ff00
    Green3 = 34, // #00af00
    Green4 = 40, // #00d700
    Green5 = 28, // #008700
    GreenYellow = 154, // #afff00
    Grey = 8, // #808080
    Grey0 = 16, // #000000
    Grey100 = 231, // #ffffff
    Grey11 = 234, // #1c1c1c
    Grey15 = 235, // #262626
    Grey19 = 236, // #303030
    Grey23 = 237, // #3a3a3a
    Grey27 = 238, // #444444
    Grey3 = 232, // #080808
    Grey30 = 239, // #4e4e4e
    Grey35 = 240, // #585858
    Grey37 = 59, // #5f5f5f
    Grey39 = 241, // #626262
    Grey42 = 242, // #6c6c6c
    Grey46 = 243, // #767676
    Grey50 = 244, // #808080
    Grey53 = 102, // #878787
    Grey54 = 245, // #8a8a8a
    Grey58 = 246, // #949494
    Grey62 = 247, // #9e9e9e
    Grey63 = 139, // #af87af
    Grey66 = 248, // #a8a8a8
    Grey69 = 145, // #afafaf
    Grey7 = 233, // #121212
    Grey70 = 249, // #b2b2b2
    Grey74 = 250, // #bcbcbc
    Grey78 = 251, // #c6c6c6
    Grey82 = 252, // #d0d0d0
    Grey84 = 188, // #d7d7d7
    Grey85 = 253, // #dadada
    Grey89 = 254, // #e4e4e4
    Grey93 = 255, // #eeeeee
    Honeydew2 = 194, // #d7ffd7
    HotPink1 = 205, // #ff5faf
    HotPink2 = 206, // #ff5fd7
    HotPink3 = 169, // #d75faf
    HotPink4 = 132, // #af5f87
    HotPink5 = 168, // #d75f87
    IndianRed1 = 131, // #af5f5f
    IndianRed2 = 167, // #d75f5f
    IndianRed3 = 203, // #ff5f5f
    IndianRed4 = 204, // #ff5f87
    Khaki1 = 228, // #ffff87
    Khaki3 = 185, // #d7d75f
    LightCoral = 210, // #ff8787
    LightCyan1 = 195, // #d7ffff
    LightCyan2 = 152, // #afd7d7
    LightGoldenrod1 = 227, // #ffff5f
    LightGoldenrod2 = 186, // #d7d787
    LightGoldenrod3 = 221, // #ffd75f
    LightGoldenrod4 = 222, // #ffd787
    LightGoldenrod5 = 179, // #d7af5f
    LightGreen1 = 119, // #87ff5f
    LightGreen2 = 120, // #87ff87
    LightPink1 = 217, // #ffafaf
    LightPink2 = 174, // #d78787
    LightPink3 = 95, // #875f5f
    LightSalmon1 = 216, // #ffaf87
    LightSalmon2 = 137, // #af875f
    LightSalmon3 = 173, // #d7875f
    LightSeaGreen = 37, // #00afaf
    LightSkyBlue1 = 153, // #afd7ff
    LightSkyBlue2 = 109, // #87afaf
    LightSkyBlue3 = 110, // #87afd7
    LightSlateBlue = 105, // #8787ff
    LightSlateGrey = 103, // #8787af
    LightSteelBlue = 147, // #afafff
    LightSteelBlue1 = 189, // #d7d7ff
    LightSteelBlue3 = 146, // #afafd7
    LightYellow3 = 187, // #d7d7af
    Lime = 10, // #00ff00
    Magenta1 = 201, // #ff00ff
    Magenta2 = 165, // #d700ff
    Magenta3 = 200, // #ff00d7
    Magenta4 = 127, // #af00af
    Magenta5 = 163, // #d700af
    Magenta6 = 164, // #d700d7
    Maroon = 1, // #800000
    MediumOrchid1 = 134, // #af5fd7
    MediumOrchid2 = 171, // #d75fff
    MediumOrchid3 = 207, // #ff5fff
    MediumOrchid4 = 133, // #af5faf
    MediumPurple1 = 104, // #8787d7
    MediumPurple2 = 141, // #af87ff
    MediumPurple3 = 135, // #af5fff
    MediumPurple4 = 140, // #af87d7
    MediumPurple5 = 97, // #875faf
    MediumPurple6 = 98, // #875fd7
    MediumPurple7 = 60, // #5f5f87
    MediumSpringGreen = 49, // #00ffaf
    MediumTurquoise = 80, // #5fd7d7
    MediumVioletRed = 126, // #af0087
    MistyRose1 = 224, // #ffd7d7
    MistyRose2 = 181, // #d7afaf
    NavajoWhite1 = 223, // #ffd7af
    NavajoWhite2 = 144, // #afaf87
    Navy = 4, // #000080
    NavyBlue = 17, // #00005f
    Olive = 3, // #808000
    Orange1 = 214, // #ffaf00
    Orange2 = 172, // #d78700
    Orange3 = 58, // #5f5f00
    Orange4 = 94, // #875f00
    OrangeRed = 202, // #ff5f00
    Orchid1 = 170, // #d75fd7
    Orchid2 = 213, // #ff87ff
    Orchid3 = 212, // #ff87d7
    PaleGreen1 = 121, // #87ffaf
    PaleGreen2 = 156, // #afff87
    PaleGreen3 = 114, // #87d787
    PaleGreen4 = 77, // #5fd75f
    PaleTurquoise1 = 159, // #afffff
    PaleTurquoise2 = 66, // #5f8787
    PaleVioletRed = 211, // #ff87af
    Pink1 = 218, // #ffafd7
    Pink2 = 175, // #d787af
    Plum1 = 219, // #ffafff
    Plum2 = 183, // #d7afff
    Plum3 = 176, // #d787d7
    Plum4 = 96, // #875f87
    Purple1 = 129, // #af00ff
    Purple2 = 5, // #800080
    Purple3 = 93, // #8700ff
    Purple4 = 56, // #5f00d7
    Purple5 = 54, // #5f0087
    Purple6 = 55, // #5f00af
    Red1 = 9, // #ff0000
    Red2 = 196, // #ff0000
    Red3 = 124, // #af0000
    Red4 = 160, // #d70000
    RosyBrown = 138, // #af8787
    RoyalBlue = 63, // #5f5fff
    Salmon = 209, // #ff875f
    SandyBrown = 215, // #ffaf5f
    SeaGreen1 = 84, // #5fff87
    SeaGreen2 = 85, // #5fffaf
    SeaGreen3 = 83, // #5fff5f
    SeaGreen4 = 78, // #5fd787
    Silver = 7, // #c0c0c0
    SkyBlue1 = 117, // #87d7ff
    SkyBlue2 = 111, // #87afff
    SkyBlue3 = 74, // #5fafd7
    SlateBlue1 = 99, // #875fff
    SlateBlue2 = 61, // #5f5faf
    SlateBlue3 = 62, // #5f5fd7
    SpringGreen1 = 48, // #00ff87
    SpringGreen2 = 42, // #00d787
    SpringGreen3 = 47, // #00ff5f
    SpringGreen4 = 35, // #00af5f
    SpringGreen5 = 41, // #00d75f
    SpringGreen6 = 29, // #00875f
    SteelBlue1 = 67, // #5f87af
    SteelBlue2 = 75, // #5fafff
    SteelBlue3 = 81, // #5fd7ff
    SteelBlue4 = 68, // #5f87d7
    Tan = 180, // #d7af87
    Teal = 6, // #008080
    Thistle1 = 225, // #ffd7ff
    Thistle2 = 182, // #d7afd7
    Turquoise1 = 45, // #00d7ff
    Turquoise2 = 30, // #008787
    Violet = 177, // #d787ff
    Wheat1 = 229, // #ffffaf
    Wheat2 = 101, // #87875f
    White = 15, // #ffffff
    Yellow1 = 11, // #ffff00
    Yellow2 = 226, // #ffff00
    Yellow3 = 190, // #d7ff00
    Yellow4 = 148, // #afd700
    Yellow5 = 184, // #d7d700
    Yellow6 = 100, // #878700
    Yellow7 = 106, // #87af00
}

public static class AnsiColorExtensions
{
    private static readonly Dictionary<Ansi256Color, Color> Ansi256ColorMap = new()
    {
        { Ansi256Color.Aqua, Color.FromArgb(0, 0x00, 0xff, 0xff) },
        { Ansi256Color.Aquamarine1, Color.FromArgb(0, 0x87, 0xff, 0xd7) },
        { Ansi256Color.Aquamarine2, Color.FromArgb(0, 0x5f, 0xff, 0xd7) },
        { Ansi256Color.Aquamarine3, Color.FromArgb(0, 0x5f, 0xd7, 0xaf) },
        { Ansi256Color.Black, Color.FromArgb(0, 0x00, 0x00, 0x00) },
        { Ansi256Color.Blue1, Color.FromArgb(0, 0x00, 0x00, 0xff) },
        { Ansi256Color.Blue2, Color.FromArgb(0, 0x00, 0x00, 0xff) },
        { Ansi256Color.Blue3, Color.FromArgb(0, 0x00, 0x00, 0xaf) },
        { Ansi256Color.Blue4, Color.FromArgb(0, 0x00, 0x00, 0xd7) },
        { Ansi256Color.BlueViolet, Color.FromArgb(0, 0x5f, 0x00, 0xff) },
        { Ansi256Color.CadetBlue1, Color.FromArgb(0, 0x5f, 0xaf, 0x87) },
        { Ansi256Color.CadetBlue2, Color.FromArgb(0, 0x5f, 0xaf, 0xaf) },
        { Ansi256Color.Chartreuse1, Color.FromArgb(0, 0x87, 0xff, 0x00) },
        { Ansi256Color.Chartreuse2, Color.FromArgb(0, 0x87, 0xd7, 0x00) },
        { Ansi256Color.Chartreuse3, Color.FromArgb(0, 0x5f, 0xff, 0x00) },
        { Ansi256Color.Chartreuse4, Color.FromArgb(0, 0x5f, 0xaf, 0x00) },
        { Ansi256Color.Chartreuse5, Color.FromArgb(0, 0x5f, 0xd7, 0x00) },
        { Ansi256Color.Chartreuse6, Color.FromArgb(0, 0x5f, 0x87, 0x00) },
        { Ansi256Color.CornflowerBlue, Color.FromArgb(0, 0x5f, 0x87, 0xff) },
        { Ansi256Color.Cornsilk1, Color.FromArgb(0, 0xff, 0xff, 0xd7) },
        { Ansi256Color.Cyan1, Color.FromArgb(0, 0x00, 0xff, 0xff) },
        { Ansi256Color.Cyan2, Color.FromArgb(0, 0x00, 0xff, 0xd7) },
        { Ansi256Color.Cyan3, Color.FromArgb(0, 0x00, 0xd7, 0xaf) },
        { Ansi256Color.DarkBlue, Color.FromArgb(0, 0x00, 0x00, 0x87) },
        { Ansi256Color.DarkCyan, Color.FromArgb(0, 0x00, 0xaf, 0x87) },
        { Ansi256Color.DarkGoldenrod, Color.FromArgb(0, 0xaf, 0x87, 0x00) },
        { Ansi256Color.DarkGreen, Color.FromArgb(0, 0x00, 0x5f, 0x00) },
        { Ansi256Color.DarkKhaki, Color.FromArgb(0, 0xaf, 0xaf, 0x5f) },
        { Ansi256Color.DarkMagenta1, Color.FromArgb(0, 0x87, 0x00, 0x87) },
        { Ansi256Color.DarkMagenta2, Color.FromArgb(0, 0x87, 0x00, 0xaf) },
        { Ansi256Color.DarkOliveGreen1, Color.FromArgb(0, 0xd7, 0xff, 0x5f) },
        { Ansi256Color.DarkOliveGreen2, Color.FromArgb(0, 0xd7, 0xff, 0x87) },
        { Ansi256Color.DarkOliveGreen3, Color.FromArgb(0, 0xaf, 0xff, 0x5f) },
        { Ansi256Color.DarkOliveGreen4, Color.FromArgb(0, 0x87, 0xaf, 0x5f) },
        { Ansi256Color.DarkOliveGreen5, Color.FromArgb(0, 0x87, 0xd7, 0x5f) },
        { Ansi256Color.DarkOliveGreen6, Color.FromArgb(0, 0xaf, 0xd7, 0x5f) },
        { Ansi256Color.DarkOrange1, Color.FromArgb(0, 0xff, 0x87, 0x00) },
        { Ansi256Color.DarkOrange2, Color.FromArgb(0, 0xaf, 0x5f, 0x00) },
        { Ansi256Color.DarkOrange3, Color.FromArgb(0, 0xd7, 0x5f, 0x00) },
        { Ansi256Color.DarkRed1, Color.FromArgb(0, 0x5f, 0x00, 0x00) },
        { Ansi256Color.DarkRed2, Color.FromArgb(0, 0x87, 0x00, 0x00) },
        { Ansi256Color.DarkSeaGreen1, Color.FromArgb(0, 0x87, 0xaf, 0x87) },
        { Ansi256Color.DarkSeaGreen2, Color.FromArgb(0, 0xaf, 0xff, 0xd7) },
        { Ansi256Color.DarkSeaGreen3, Color.FromArgb(0, 0xd7, 0xff, 0xaf) },
        { Ansi256Color.DarkSeaGreen4, Color.FromArgb(0, 0xaf, 0xd7, 0xaf) },
        { Ansi256Color.DarkSeaGreen5, Color.FromArgb(0, 0xaf, 0xff, 0xaf) },
        { Ansi256Color.DarkSeaGreen6, Color.FromArgb(0, 0x87, 0xd7, 0xaf) },
        { Ansi256Color.DarkSeaGreen7, Color.FromArgb(0, 0xaf, 0xd7, 0x87) },
        { Ansi256Color.DarkSeaGreen8, Color.FromArgb(0, 0x5f, 0x87, 0x5f) },
        { Ansi256Color.DarkSeaGreen9, Color.FromArgb(0, 0x5f, 0xaf, 0x5f) },
        { Ansi256Color.DarkSlateGray1, Color.FromArgb(0, 0x87, 0xff, 0xff) },
        { Ansi256Color.DarkSlateGray2, Color.FromArgb(0, 0x5f, 0xff, 0xff) },
        { Ansi256Color.DarkSlateGray3, Color.FromArgb(0, 0x87, 0xd7, 0xd7) },
        { Ansi256Color.DarkTurquoise, Color.FromArgb(0, 0x00, 0xd7, 0xd7) },
        { Ansi256Color.DarkViolet1, Color.FromArgb(0, 0xaf, 0x00, 0xd7) },
        { Ansi256Color.DarkViolet2, Color.FromArgb(0, 0x87, 0x00, 0xd7) },
        { Ansi256Color.DeepPink1, Color.FromArgb(0, 0xff, 0x00, 0x87) },
        { Ansi256Color.DeepPink2, Color.FromArgb(0, 0xff, 0x00, 0xaf) },
        { Ansi256Color.DeepPink3, Color.FromArgb(0, 0xff, 0x00, 0x5f) },
        { Ansi256Color.DeepPink4, Color.FromArgb(0, 0xd7, 0x00, 0x5f) },
        { Ansi256Color.DeepPink5, Color.FromArgb(0, 0xd7, 0x00, 0x87) },
        { Ansi256Color.DeepPink6, Color.FromArgb(0, 0xaf, 0x00, 0x5f) },
        { Ansi256Color.DeepPink7, Color.FromArgb(0, 0x5f, 0x00, 0x5f) },
        { Ansi256Color.DeepPink8, Color.FromArgb(0, 0x87, 0x00, 0x5f) },
        { Ansi256Color.DeepSkyBlue1, Color.FromArgb(0, 0x00, 0xaf, 0xff) },
        { Ansi256Color.DeepSkyBlue2, Color.FromArgb(0, 0x00, 0xaf, 0xd7) },
        { Ansi256Color.DeepSkyBlue3, Color.FromArgb(0, 0x00, 0x87, 0xaf) },
        { Ansi256Color.DeepSkyBlue4, Color.FromArgb(0, 0x00, 0x87, 0xd7) },
        { Ansi256Color.DeepSkyBlue5, Color.FromArgb(0, 0x00, 0x5f, 0x5f) },
        { Ansi256Color.DeepSkyBlue6, Color.FromArgb(0, 0x00, 0x5f, 0x87) },
        { Ansi256Color.DeepSkyBlue7, Color.FromArgb(0, 0x00, 0x5f, 0xaf) },
        { Ansi256Color.DodgerBlue1, Color.FromArgb(0, 0x00, 0x87, 0xff) },
        { Ansi256Color.DodgerBlue2, Color.FromArgb(0, 0x00, 0x5f, 0xff) },
        { Ansi256Color.DodgerBlue3, Color.FromArgb(0, 0x00, 0x5f, 0xd7) },
        { Ansi256Color.Fuchsia, Color.FromArgb(0, 0xff, 0x00, 0xff) },
        { Ansi256Color.Gold1, Color.FromArgb(0, 0xff, 0xd7, 0x00) },
        { Ansi256Color.Gold2, Color.FromArgb(0, 0xaf, 0xaf, 0x00) },
        { Ansi256Color.Gold3, Color.FromArgb(0, 0xd7, 0xaf, 0x00) },
        { Ansi256Color.Green1, Color.FromArgb(0, 0x00, 0x80, 0x00) },
        { Ansi256Color.Green2, Color.FromArgb(0, 0x00, 0xff, 0x00) },
        { Ansi256Color.Green3, Color.FromArgb(0, 0x00, 0xaf, 0x00) },
        { Ansi256Color.Green4, Color.FromArgb(0, 0x00, 0xd7, 0x00) },
        { Ansi256Color.Green5, Color.FromArgb(0, 0x00, 0x87, 0x00) },
        { Ansi256Color.GreenYellow, Color.FromArgb(0, 0xaf, 0xff, 0x00) },
        { Ansi256Color.Grey, Color.FromArgb(0, 0x80, 0x80, 0x80) },
        { Ansi256Color.Grey0, Color.FromArgb(0, 0x00, 0x00, 0x00) },
        { Ansi256Color.Grey100, Color.FromArgb(0, 0xff, 0xff, 0xff) },
        { Ansi256Color.Grey11, Color.FromArgb(0, 0x1c, 0x1c, 0x1c) },
        { Ansi256Color.Grey15, Color.FromArgb(0, 0x26, 0x26, 0x26) },
        { Ansi256Color.Grey19, Color.FromArgb(0, 0x30, 0x30, 0x30) },
        { Ansi256Color.Grey23, Color.FromArgb(0, 0x3a, 0x3a, 0x3a) },
        { Ansi256Color.Grey27, Color.FromArgb(0, 0x44, 0x44, 0x44) },
        { Ansi256Color.Grey3, Color.FromArgb(0, 0x08, 0x08, 0x08) },
        { Ansi256Color.Grey30, Color.FromArgb(0, 0x4e, 0x4e, 0x4e) },
        { Ansi256Color.Grey35, Color.FromArgb(0, 0x58, 0x58, 0x58) },
        { Ansi256Color.Grey37, Color.FromArgb(0, 0x5f, 0x5f, 0x5f) },
        { Ansi256Color.Grey39, Color.FromArgb(0, 0x62, 0x62, 0x62) },
        { Ansi256Color.Grey42, Color.FromArgb(0, 0x6c, 0x6c, 0x6c) },
        { Ansi256Color.Grey46, Color.FromArgb(0, 0x76, 0x76, 0x76) },
        { Ansi256Color.Grey50, Color.FromArgb(0, 0x80, 0x80, 0x80) },
        { Ansi256Color.Grey53, Color.FromArgb(0, 0x87, 0x87, 0x87) },
        { Ansi256Color.Grey54, Color.FromArgb(0, 0x8a, 0x8a, 0x8a) },
        { Ansi256Color.Grey58, Color.FromArgb(0, 0x94, 0x94, 0x94) },
        { Ansi256Color.Grey62, Color.FromArgb(0, 0x9e, 0x9e, 0x9e) },
        { Ansi256Color.Grey63, Color.FromArgb(0, 0xaf, 0x87, 0xaf) },
        { Ansi256Color.Grey66, Color.FromArgb(0, 0xa8, 0xa8, 0xa8) },
        { Ansi256Color.Grey69, Color.FromArgb(0, 0xaf, 0xaf, 0xaf) },
        { Ansi256Color.Grey7, Color.FromArgb(0, 0x12, 0x12, 0x12) },
        { Ansi256Color.Grey70, Color.FromArgb(0, 0xb2, 0xb2, 0xb2) },
        { Ansi256Color.Grey74, Color.FromArgb(0, 0xbc, 0xbc, 0xbc) },
        { Ansi256Color.Grey78, Color.FromArgb(0, 0xc6, 0xc6, 0xc6) },
        { Ansi256Color.Grey82, Color.FromArgb(0, 0xd0, 0xd0, 0xd0) },
        { Ansi256Color.Grey84, Color.FromArgb(0, 0xd7, 0xd7, 0xd7) },
        { Ansi256Color.Grey85, Color.FromArgb(0, 0xda, 0xda, 0xda) },
        { Ansi256Color.Grey89, Color.FromArgb(0, 0xe4, 0xe4, 0xe4) },
        { Ansi256Color.Grey93, Color.FromArgb(0, 0xee, 0xee, 0xee) },
        { Ansi256Color.Honeydew2, Color.FromArgb(0, 0xd7, 0xff, 0xd7) },
        { Ansi256Color.HotPink1, Color.FromArgb(0, 0xff, 0x5f, 0xaf) },
        { Ansi256Color.HotPink2, Color.FromArgb(0, 0xff, 0x5f, 0xd7) },
        { Ansi256Color.HotPink3, Color.FromArgb(0, 0xd7, 0x5f, 0xaf) },
        { Ansi256Color.HotPink4, Color.FromArgb(0, 0xaf, 0x5f, 0x87) },
        { Ansi256Color.HotPink5, Color.FromArgb(0, 0xd7, 0x5f, 0x87) },
        { Ansi256Color.IndianRed1, Color.FromArgb(0, 0xaf, 0x5f, 0x5f) },
        { Ansi256Color.IndianRed2, Color.FromArgb(0, 0xd7, 0x5f, 0x5f) },
        { Ansi256Color.IndianRed3, Color.FromArgb(0, 0xff, 0x5f, 0x5f) },
        { Ansi256Color.IndianRed4, Color.FromArgb(0, 0xff, 0x5f, 0x87) },
        { Ansi256Color.Khaki1, Color.FromArgb(0, 0xff, 0xff, 0x87) },
        { Ansi256Color.Khaki3, Color.FromArgb(0, 0xd7, 0xd7, 0x5f) },
        { Ansi256Color.LightCoral, Color.FromArgb(0, 0xff, 0x87, 0x87) },
        { Ansi256Color.LightCyan1, Color.FromArgb(0, 0xd7, 0xff, 0xff) },
        { Ansi256Color.LightCyan2, Color.FromArgb(0, 0xaf, 0xd7, 0xd7) },
        { Ansi256Color.LightGoldenrod1, Color.FromArgb(0, 0xff, 0xff, 0x5f) },
        { Ansi256Color.LightGoldenrod2, Color.FromArgb(0, 0xd7, 0xd7, 0x87) },
        { Ansi256Color.LightGoldenrod3, Color.FromArgb(0, 0xff, 0xd7, 0x5f) },
        { Ansi256Color.LightGoldenrod4, Color.FromArgb(0, 0xff, 0xd7, 0x87) },
        { Ansi256Color.LightGoldenrod5, Color.FromArgb(0, 0xd7, 0xaf, 0x5f) },
        { Ansi256Color.LightGreen1, Color.FromArgb(0, 0x87, 0xff, 0x5f) },
        { Ansi256Color.LightGreen2, Color.FromArgb(0, 0x87, 0xff, 0x87) },
        { Ansi256Color.LightPink1, Color.FromArgb(0, 0xff, 0xaf, 0xaf) },
        { Ansi256Color.LightPink2, Color.FromArgb(0, 0xd7, 0x87, 0x87) },
        { Ansi256Color.LightPink3, Color.FromArgb(0, 0x87, 0x5f, 0x5f) },
        { Ansi256Color.LightSalmon1, Color.FromArgb(0, 0xff, 0xaf, 0x87) },
        { Ansi256Color.LightSalmon2, Color.FromArgb(0, 0xaf, 0x87, 0x5f) },
        { Ansi256Color.LightSalmon3, Color.FromArgb(0, 0xd7, 0x87, 0x5f) },
        { Ansi256Color.LightSeaGreen, Color.FromArgb(0, 0x00, 0xaf, 0xaf) },
        { Ansi256Color.LightSkyBlue1, Color.FromArgb(0, 0xaf, 0xd7, 0xff) },
        { Ansi256Color.LightSkyBlue2, Color.FromArgb(0, 0x87, 0xaf, 0xaf) },
        { Ansi256Color.LightSkyBlue3, Color.FromArgb(0, 0x87, 0xaf, 0xd7) },
        { Ansi256Color.LightSlateBlue, Color.FromArgb(0, 0x87, 0x87, 0xff) },
        { Ansi256Color.LightSlateGrey, Color.FromArgb(0, 0x87, 0x87, 0xaf) },
        { Ansi256Color.LightSteelBlue, Color.FromArgb(0, 0xaf, 0xaf, 0xff) },
        { Ansi256Color.LightSteelBlue1, Color.FromArgb(0, 0xd7, 0xd7, 0xff) },
        { Ansi256Color.LightSteelBlue3, Color.FromArgb(0, 0xaf, 0xaf, 0xd7) },
        { Ansi256Color.LightYellow3, Color.FromArgb(0, 0xd7, 0xd7, 0xaf) },
        { Ansi256Color.Lime, Color.FromArgb(0, 0x00, 0xff, 0x00) },
        { Ansi256Color.Magenta1, Color.FromArgb(0, 0xff, 0x00, 0xff) },
        { Ansi256Color.Magenta2, Color.FromArgb(0, 0xd7, 0x00, 0xff) },
        { Ansi256Color.Magenta3, Color.FromArgb(0, 0xff, 0x00, 0xd7) },
        { Ansi256Color.Magenta4, Color.FromArgb(0, 0xaf, 0x00, 0xaf) },
        { Ansi256Color.Magenta5, Color.FromArgb(0, 0xd7, 0x00, 0xaf) },
        { Ansi256Color.Magenta6, Color.FromArgb(0, 0xd7, 0x00, 0xd7) },
        { Ansi256Color.Maroon, Color.FromArgb(0, 0x80, 0x00, 0x00) },
        { Ansi256Color.MediumOrchid1, Color.FromArgb(0, 0xaf, 0x5f, 0xd7) },
        { Ansi256Color.MediumOrchid2, Color.FromArgb(0, 0xd7, 0x5f, 0xff) },
        { Ansi256Color.MediumOrchid3, Color.FromArgb(0, 0xff, 0x5f, 0xff) },
        { Ansi256Color.MediumOrchid4, Color.FromArgb(0, 0xaf, 0x5f, 0xaf) },
        { Ansi256Color.MediumPurple1, Color.FromArgb(0, 0x87, 0x87, 0xd7) },
        { Ansi256Color.MediumPurple2, Color.FromArgb(0, 0xaf, 0x87, 0xff) },
        { Ansi256Color.MediumPurple3, Color.FromArgb(0, 0xaf, 0x5f, 0xff) },
        { Ansi256Color.MediumPurple4, Color.FromArgb(0, 0xaf, 0x87, 0xd7) },
        { Ansi256Color.MediumPurple5, Color.FromArgb(0, 0x87, 0x5f, 0xaf) },
        { Ansi256Color.MediumPurple6, Color.FromArgb(0, 0x87, 0x5f, 0xd7) },
        { Ansi256Color.MediumPurple7, Color.FromArgb(0, 0x5f, 0x5f, 0x87) },
        { Ansi256Color.MediumSpringGreen, Color.FromArgb(0, 0x00, 0xff, 0xaf) },
        { Ansi256Color.MediumTurquoise, Color.FromArgb(0, 0x5f, 0xd7, 0xd7) },
        { Ansi256Color.MediumVioletRed, Color.FromArgb(0, 0xaf, 0x00, 0x87) },
        { Ansi256Color.MistyRose1, Color.FromArgb(0, 0xff, 0xd7, 0xd7) },
        { Ansi256Color.MistyRose2, Color.FromArgb(0, 0xd7, 0xaf, 0xaf) },
        { Ansi256Color.NavajoWhite1, Color.FromArgb(0, 0xff, 0xd7, 0xaf) },
        { Ansi256Color.NavajoWhite2, Color.FromArgb(0, 0xaf, 0xaf, 0x87) },
        { Ansi256Color.Navy, Color.FromArgb(0, 0x00, 0x00, 0x80) },
        { Ansi256Color.NavyBlue, Color.FromArgb(0, 0x00, 0x00, 0x5f) },
        { Ansi256Color.Olive, Color.FromArgb(0, 0x80, 0x80, 0x00) },
        { Ansi256Color.Orange1, Color.FromArgb(0, 0xff, 0xaf, 0x00) },
        { Ansi256Color.Orange2, Color.FromArgb(0, 0xd7, 0x87, 0x00) },
        { Ansi256Color.Orange3, Color.FromArgb(0, 0x5f, 0x5f, 0x00) },
        { Ansi256Color.Orange4, Color.FromArgb(0, 0x87, 0x5f, 0x00) },
        { Ansi256Color.OrangeRed, Color.FromArgb(0, 0xff, 0x5f, 0x00) },
        { Ansi256Color.Orchid1, Color.FromArgb(0, 0xd7, 0x5f, 0xd7) },
        { Ansi256Color.Orchid2, Color.FromArgb(0, 0xff, 0x87, 0xff) },
        { Ansi256Color.Orchid3, Color.FromArgb(0, 0xff, 0x87, 0xd7) },
        { Ansi256Color.PaleGreen1, Color.FromArgb(0, 0x87, 0xff, 0xaf) },
        { Ansi256Color.PaleGreen2, Color.FromArgb(0, 0xaf, 0xff, 0x87) },
        { Ansi256Color.PaleGreen3, Color.FromArgb(0, 0x87, 0xd7, 0x87) },
        { Ansi256Color.PaleGreen4, Color.FromArgb(0, 0x5f, 0xd7, 0x5f) },
        { Ansi256Color.PaleTurquoise1, Color.FromArgb(0, 0xaf, 0xff, 0xff) },
        { Ansi256Color.PaleTurquoise2, Color.FromArgb(0, 0x5f, 0x87, 0x87) },
        { Ansi256Color.PaleVioletRed, Color.FromArgb(0, 0xff, 0x87, 0xaf) },
        { Ansi256Color.Pink1, Color.FromArgb(0, 0xff, 0xaf, 0xd7) },
        { Ansi256Color.Pink2, Color.FromArgb(0, 0xd7, 0x87, 0xaf) },
        { Ansi256Color.Plum1, Color.FromArgb(0, 0xff, 0xaf, 0xff) },
        { Ansi256Color.Plum2, Color.FromArgb(0, 0xd7, 0xaf, 0xff) },
        { Ansi256Color.Plum3, Color.FromArgb(0, 0xd7, 0x87, 0xd7) },
        { Ansi256Color.Plum4, Color.FromArgb(0, 0x87, 0x5f, 0x87) },
        { Ansi256Color.Purple1, Color.FromArgb(0, 0xaf, 0x00, 0xff) },
        { Ansi256Color.Purple2, Color.FromArgb(0, 0x80, 0x00, 0x80) },
        { Ansi256Color.Purple3, Color.FromArgb(0, 0x87, 0x00, 0xff) },
        { Ansi256Color.Purple4, Color.FromArgb(0, 0x5f, 0x00, 0xd7) },
        { Ansi256Color.Purple5, Color.FromArgb(0, 0x5f, 0x00, 0x87) },
        { Ansi256Color.Purple6, Color.FromArgb(0, 0x5f, 0x00, 0xaf) },
        { Ansi256Color.Red1, Color.FromArgb(0, 0xff, 0x00, 0x00) },
        { Ansi256Color.Red2, Color.FromArgb(0, 0xff, 0x00, 0x00) },
        { Ansi256Color.Red3, Color.FromArgb(0, 0xaf, 0x00, 0x00) },
        { Ansi256Color.Red4, Color.FromArgb(0, 0xd7, 0x00, 0x00) },
        { Ansi256Color.RosyBrown, Color.FromArgb(0, 0xaf, 0x87, 0x87) },
        { Ansi256Color.RoyalBlue, Color.FromArgb(0, 0x5f, 0x5f, 0xff) },
        { Ansi256Color.Salmon, Color.FromArgb(0, 0xff, 0x87, 0x5f) },
        { Ansi256Color.SandyBrown, Color.FromArgb(0, 0xff, 0xaf, 0x5f) },
        { Ansi256Color.SeaGreen1, Color.FromArgb(0, 0x5f, 0xff, 0x87) },
        { Ansi256Color.SeaGreen2, Color.FromArgb(0, 0x5f, 0xff, 0xaf) },
        { Ansi256Color.SeaGreen3, Color.FromArgb(0, 0x5f, 0xff, 0x5f) },
        { Ansi256Color.SeaGreen4, Color.FromArgb(0, 0x5f, 0xd7, 0x87) },
        { Ansi256Color.Silver, Color.FromArgb(0, 0xc0, 0xc0, 0xc0) },
        { Ansi256Color.SkyBlue1, Color.FromArgb(0, 0x87, 0xd7, 0xff) },
        { Ansi256Color.SkyBlue2, Color.FromArgb(0, 0x87, 0xaf, 0xff) },
        { Ansi256Color.SkyBlue3, Color.FromArgb(0, 0x5f, 0xaf, 0xd7) },
        { Ansi256Color.SlateBlue1, Color.FromArgb(0, 0x87, 0x5f, 0xff) },
        { Ansi256Color.SlateBlue2, Color.FromArgb(0, 0x5f, 0x5f, 0xaf) },
        { Ansi256Color.SlateBlue3, Color.FromArgb(0, 0x5f, 0x5f, 0xd7) },
        { Ansi256Color.SpringGreen1, Color.FromArgb(0, 0x00, 0xff, 0x87) },
        { Ansi256Color.SpringGreen2, Color.FromArgb(0, 0x00, 0xd7, 0x87) },
        { Ansi256Color.SpringGreen3, Color.FromArgb(0, 0x00, 0xff, 0x5f) },
        { Ansi256Color.SpringGreen4, Color.FromArgb(0, 0x00, 0xaf, 0x5f) },
        { Ansi256Color.SpringGreen5, Color.FromArgb(0, 0x00, 0xd7, 0x5f) },
        { Ansi256Color.SpringGreen6, Color.FromArgb(0, 0x00, 0x87, 0x5f) },
        { Ansi256Color.SteelBlue1, Color.FromArgb(0, 0x5f, 0x87, 0xaf) },
        { Ansi256Color.SteelBlue2, Color.FromArgb(0, 0x5f, 0xaf, 0xff) },
        { Ansi256Color.SteelBlue3, Color.FromArgb(0, 0x5f, 0xd7, 0xff) },
        { Ansi256Color.SteelBlue4, Color.FromArgb(0, 0x5f, 0x87, 0xd7) },
        { Ansi256Color.Tan, Color.FromArgb(0, 0xd7, 0xaf, 0x87) },
        { Ansi256Color.Teal, Color.FromArgb(0, 0x00, 0x80, 0x80) },
        { Ansi256Color.Thistle1, Color.FromArgb(0, 0xff, 0xd7, 0xff) },
        { Ansi256Color.Thistle2, Color.FromArgb(0, 0xd7, 0xaf, 0xd7) },
        { Ansi256Color.Turquoise1, Color.FromArgb(0, 0x00, 0xd7, 0xff) },
        { Ansi256Color.Turquoise2, Color.FromArgb(0, 0x00, 0x87, 0x87) },
        { Ansi256Color.Violet, Color.FromArgb(0, 0xd7, 0x87, 0xff) },
        { Ansi256Color.Wheat1, Color.FromArgb(0, 0xff, 0xff, 0xaf) },
        { Ansi256Color.Wheat2, Color.FromArgb(0, 0x87, 0x87, 0x5f) },
        { Ansi256Color.White, Color.FromArgb(0, 0xff, 0xff, 0xff) },
        { Ansi256Color.Yellow1, Color.FromArgb(0, 0xff, 0xff, 0x00) },
        { Ansi256Color.Yellow2, Color.FromArgb(0, 0xff, 0xff, 0x00) },
        { Ansi256Color.Yellow3, Color.FromArgb(0, 0xd7, 0xff, 0x00) },
        { Ansi256Color.Yellow4, Color.FromArgb(0, 0xaf, 0xd7, 0x00) },
        { Ansi256Color.Yellow5, Color.FromArgb(0, 0xd7, 0xd7, 0x00) },
        { Ansi256Color.Yellow6, Color.FromArgb(0, 0x87, 0x87, 0x00) },
        { Ansi256Color.Yellow7, Color.FromArgb(0, 0x87, 0xaf, 0x00) },
    };
    
    public static Color GetColor(this Ansi256Color color) => Ansi256ColorMap[color];

    public static Ansi256Color ClosestAnsi256(this Color color)
    {
        var minDelta = int.MaxValue;
        Ansi256Color closest = default;
        foreach (var colorPair in Ansi256ColorMap)
        {
            var r = Math.Abs(colorPair.Value.R - color.R);
            var g = Math.Abs(colorPair.Value.G - color.G);
            var b = Math.Abs(colorPair.Value.B - color.B);
            var delta = r + g + b;
            if (delta >= minDelta) continue;
            minDelta = delta;
            closest = colorPair.Key;
        }
        return closest;
    }
}