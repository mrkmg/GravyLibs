namespace Gravy.MetaString;

public partial class MetaString<T>
{
    public PositionedMetaEntry<T>[] MetaEntries { get; }
    public string RawText => string.Join(null, MetaEntries.Select(e => e.Text));

    public MetaString(string text, T metaData)
    {
        MetaEntries = new [] { new PositionedMetaEntry<T>(0, text, metaData) };
    }

    protected MetaString(IEnumerable<MetaEntry<T>> entries)
    {
        MetaEntries = entries.Positioned().ToArray();
    }

    protected MetaString(PositionedMetaEntry<T>[] entries)
    {
        MetaEntries = entries;
    }

    public object Clone()
        => new MetaString<T>(MetaEntries);

    public MetaChar<T> this[int index] => CharAt(index);

    public virtual MetaString<T> this[Range range] => GetRange(range);
    
    public MetaChar<T> CharAt(int index)
    {
        var (entryIdx, charInx) = GetIndexes(index);
        var entry = MetaEntries[entryIdx];
        return new (entry.Text[charInx], entry.Data);
    }

    private void CheckPositions()
    {
        var current = 0;
        for (var i = 0; i < MetaEntries.Length; i++)
        {
            if (MetaEntries[i].Offset != current)
                throw new ArgumentException("The offset did not match the expected value");
            current += MetaEntries[i].Length;
        }
    }

    private MetaString<T> GetRange(Range range)
    {
        var (startEntryIndex, startCharIndex) = GetIndexes(range.Start);
        var (endEntryIndex, endCharIndex) = GetIndexes(range.End);
        
        if (endEntryIndex < startEntryIndex || (startEntryIndex == endEntryIndex && endCharIndex < startCharIndex))
            throw new ArgumentException("Invalid range, end index is less than start index", nameof(range));
        
        if (endEntryIndex - startEntryIndex == 0)
            return new(MetaEntries[startEntryIndex].Text[startCharIndex..endCharIndex], MetaEntries[startEntryIndex].Data!);
        
        var entryIndexDifference = endEntryIndex - startEntryIndex;

        var entries = new MetaEntry<T>[entryIndexDifference + 1];
        entries[0] = Transform(MetaEntries[startEntryIndex], new(startCharIndex, new(0, true)));
        int i;
        for (i = 1; i + startEntryIndex < endEntryIndex; i++) entries[i] = MetaEntries[i + startEntryIndex];
        entries[i] = Transform(MetaEntries[i + startEntryIndex], new(0, endCharIndex));
        return new(entries.Positioned());
    }

    private static MetaEntry<T> Transform(PositionedMetaEntry<T> entry, Range range)
        => new(entry.Text[range], entry.Data);

    private (int entryIndex, int charIndex) GetIndexes(Index index)
    {
        var usedChars = 0;
        var entryIndex = index.IsFromEnd ? MetaEntries.Length - 1 : 0;
        while (true)
        {
            if (entryIndex == MetaEntries.Length || entryIndex == -1) throw new IndexOutOfRangeException();
            var length = MetaEntries[entryIndex].Length;
            if (usedChars + length > index.Value)
                break;
            usedChars += length; 
            entryIndex += index.IsFromEnd ? -1 : 1;
        }
        var charIndex = index.IsFromEnd ? MetaEntries[entryIndex].Length - (index.Value - usedChars) : index.Value - usedChars;
        return (entryIndex, charIndex);
    }
}

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
}

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
        
    public static EqualityComparer<T> Comparer { get; set; } = EqualityComparer<T>.Default;

    public static bool operator ==(MetaEntry<T> left, MetaEntry<T> right)
        => left.Equals(right);

    public static bool operator !=(MetaEntry<T> left, MetaEntry<T> right)
        => !(left == right);
}

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

    public MetaEntry<T> WithoutPosition()
        => new(Text, Data);

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

public static class PositionedMetaEntryExtensions
{
    internal static void Reposition<T>(this PositionedMetaEntry<T>[] entries)
    {
        var next = 0;
        for (var i = 0; i < entries.Length; i++)
        {
            entries[i] = new(next, entries[i].Text, entries[i].Data);
            next += entries[i].Length;
        }
    }
    
    public static PositionedMetaEntry<T>[] Positioned<T>(this IList<MetaEntry<T>> entries)
    {
        var next = 0;
        var positioned = new PositionedMetaEntry<T>[entries.Count];
        for (var i = 0; i < entries.Count; i++)
        {
            positioned[i] = new(next, entries[i].Text, entries[i].Data);
            next += entries[i].Length;
        }
        return positioned;
    }
    
    public static IEnumerable<PositionedMetaEntry<T>> Positioned<T>(this IEnumerable<MetaEntry<T>> entries)
    {
        var next = 0;
        foreach (var entry in entries)
        {
            yield return new(next, entry.Text, entry.Data);
            next += entry.Length;
        }
    }
    
    public static PositionedMetaEntry<T> PositionedAt<T>(this PositionedMetaEntry<T> entity, int offset)
        => new(offset, entity.Text, entity.Data);
}