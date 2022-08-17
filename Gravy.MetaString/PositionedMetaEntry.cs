namespace Gravy.MetaString;

public readonly struct PositionedMetaEntry<T>
{
    public PositionedMetaEntry(int offset, string text, T? data)
    {
        Offset = offset;
        Text = text;
        Data = data;
    }

    public readonly int Offset;
    public readonly string Text;
    public readonly T? Data;
    
    public int Length => Text.Length;
    public Range Range => new(Offset, Offset + Length);

    public MetaEntry<T> WithoutPosition() => new(Text, Data);

    public bool Equivalent(PositionedMetaEntry<T> other)
        => Text == other.Text && DataEquals(other);

    public bool DataEquals(PositionedMetaEntry<T> other)
        => MetaEntry<T>.Comparer.Equals(Data, other.Data);

    public bool Equals(PositionedMetaEntry<T> other)
        => Offset == other.Offset && Text == other.Text && DataEquals(other);

    public override bool Equals(object? obj)
        => obj is PositionedMetaEntry<T> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Offset, Text, Data);

    public static bool operator ==(PositionedMetaEntry<T> left, PositionedMetaEntry<T> right)
        => left.Equals(right);

    public static bool operator !=(PositionedMetaEntry<T> left, PositionedMetaEntry<T> right)
        => !(left == right);
    
    public static implicit operator MetaEntry<T>(PositionedMetaEntry<T> entry) 
        => entry.WithoutPosition();
}