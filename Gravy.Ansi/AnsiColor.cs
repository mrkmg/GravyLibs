using System.Drawing;
using JetBrains.Annotations;

namespace Gravy.Ansi;

[PublicAPI]
public readonly struct AnsiColor
{
    public readonly AnsiColorType Type;
    public readonly Ansi16Color Ansi16Color;
    public readonly Ansi256Color Ansi256Color;
    public readonly Color RgbColor;

    public AnsiColor(Ansi16Color ansi16Color)
    {
        Ansi16Color = ansi16Color;
        Ansi256Color = default;
        RgbColor = default;
        Type = AnsiColorType.Ansi16;
    }

    public AnsiColor(Ansi256Color ansi256Color)
    {
        Ansi16Color = default;
        Ansi256Color = ansi256Color;
        RgbColor = default;
        Type = AnsiColorType.Ansi256;
    }

    public AnsiColor(Color color)
    {
        if (color.A != 0xFF)
            throw new ArgumentException("Transparency is not supported", nameof(color));
        Ansi16Color = default;
        Ansi256Color = default;
        RgbColor = color;
        Type = AnsiColorType.Rgb;
    }

    public AnsiColor(int ansi256Color) : this((Ansi256Color)ansi256Color)
    {
        if (ansi256Color is < 0 or > 255)
            throw new ArgumentOutOfRangeException(nameof(ansi256Color), ansi256Color, "Must be between 0 and 255");
    }

    public AnsiColor(byte r, byte g, byte b) : this(Color.FromArgb(0xFF, r, g, b)) { }

    public bool Equals(AnsiColor other) 
        => Type switch {
            AnsiColorType.Ansi256 => other.Type == AnsiColorType.Ansi256 && Ansi256Color == other.Ansi256Color,
            AnsiColorType.Ansi16 => other.Type == AnsiColorType.Ansi16 && Ansi16Color == other.Ansi16Color,
            AnsiColorType.Rgb => other.Type == AnsiColorType.Rgb && RgbColor == other.RgbColor,
            _ => throw new ArgumentOutOfRangeException(),
        };

    public bool Equivalent(AnsiColor other)
        => Type switch
        {
            AnsiColorType.Rgb => other.Type == AnsiColorType.Rgb && RgbColor.ToArgb() == other.RgbColor.ToArgb(),
            _ => Equals(other),
        };

    public override bool Equals(object? obj) => obj is AnsiColor other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Type, Ansi256Color, Ansi16Color, RgbColor);
    public static bool operator ==(AnsiColor left, AnsiColor right) => left.Equals(right);
    public static bool operator !=(AnsiColor left, AnsiColor right) => !left.Equals(right);
    
    public static implicit operator AnsiColor(Ansi16Color color) => new(color);
    public static implicit operator AnsiColor(Color color) => new(color);
    public static implicit operator AnsiColor(Ansi256Color color) => new(color);
}


public enum AnsiColorType
{
    Ansi16,
    Ansi256,
    Rgb,
}