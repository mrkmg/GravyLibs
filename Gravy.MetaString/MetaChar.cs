namespace Gravy.MetaString;

public readonly struct MetaChar<T>
{
    public MetaChar(char text, T? data)
    {
        Char = text;
        Data = data;
    }

    public readonly char Char;
    public readonly T? Data;

    public bool DataEquals(MetaChar<T> other)
    {
        return Comparer.Equals(Data, other.Data);
    }
        
    public bool Equals(MetaChar<T> other)
        => Char == other.Char && DataEquals(other);

    public override bool Equals(object? obj)
        => obj is PositionedMetaEntry<T> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Char, Data);
        
    private static EqualityComparer<T> Comparer { get; } = EqualityComparer<T>.Default;

    public static bool operator ==(MetaChar<T> left, MetaChar<T> right)
        => left.Equals(right);

    public static bool operator !=(MetaChar<T> left, MetaChar<T> right)
        => !(left == right);

    public static implicit operator char(MetaChar<T> metaChar)
        => metaChar.Char;
}