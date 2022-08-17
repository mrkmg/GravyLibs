namespace Gravy.MetaString;

public partial class MetaString<T>
{
    private bool Equals(MetaString<T> other)
        => MetaEntries.SequenceEqual(other.MetaEntries);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        if (obj is MetaString<T> metaString) return Equals(metaString);
        return false;
    }

    public override int GetHashCode()
        => MetaEntries.GetHashCode();

    public static bool operator ==(MetaString<T> first, MetaString<T> second)
        => first.Equals(second);

    public static bool operator !=(MetaString<T> first, MetaString<T> second)
        => !first.Equals(second);

    public static MetaString<T> operator +(MetaString<T> first, MetaString<T> second)
    {
        if (first.MetaEntries.Length == 0) return second;
        if (second.MetaEntries.Length == 0) return first;
        var totalLength = first.MetaEntries.Length + second.MetaEntries.Length;
        var entries = new PositionedMetaEntry<T>[totalLength];
        Array.Copy(first.MetaEntries, entries, first.MetaEntries.Length);
        Array.Copy(second.MetaEntries, 0, entries, first.MetaEntries.Length, second.MetaEntries.Length);
        entries.Reposition();
        return new(entries);
    }

    public static MetaString<T> operator +(MetaString<T> first, string second) 
        => first + new MetaString<T>(second);

    public static MetaString<T> operator +(string first, MetaString<T> second) 
        => new MetaString<T>(first) + second;

    public static MetaString<T> operator +(MetaString<T> first, object second)
        => first + new MetaString<T>(second.ToString() ?? second.GetType().Name);

    public static MetaString<T> operator +(object first, MetaString<T> second)
        => new MetaString<T>(first.ToString() ?? first.GetType().Name) + second;
    
    public static implicit operator MetaString<T>(string str)
        => new(str, default(T)!);
}