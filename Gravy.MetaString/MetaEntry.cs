namespace Gravy.MetaString;

public readonly struct MetaEntry<T>
{
    public MetaEntry(string text, T? data)
    {
        Text = text;
        Data = data;
    }

    public readonly string Text;
    public readonly T? Data;
    
    public int Length => Text.Length;

    public PositionedMetaEntry<T> Positioned(int offset)
        => new(offset, Text, Data);

    public bool DataEquals(MetaEntry<T> other)
        => Comparer.Equals(Data, other.Data);

    public bool Equals(MetaEntry<T> other)
        => Text == other.Text && DataEquals(other);

    public override bool Equals(object? obj)
        => obj is MetaEntry<T> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Text, Data);
        
    public static IEqualityComparer<T> Comparer { get; set; } = EqualityComparer<T>.Default;

    public static bool operator ==(MetaEntry<T> left, MetaEntry<T> right)
        => left.Equals(right);

    public static bool operator !=(MetaEntry<T> left, MetaEntry<T> right)
        => !(left == right);
}