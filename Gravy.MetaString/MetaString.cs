using System.Diagnostics;

namespace Gravy.MetaString;

[DebuggerDisplay("{DebuggerDisplay}")]
public partial class MetaString<T>
{
    protected PositionedMetaEntry<T>[] MetaEntries { get; }
    
    public string RawText => string.Join(null, MetaEntries.Select(e => e.Text));
    public IEnumerable<PositionedMetaEntry<T>> MetaData => MetaEntries;
    public MetaString(string text)
    {
        MetaEntries = new [] { new PositionedMetaEntry<T>(0, text, default) };
    }
    
    public MetaString(string text, T metaData = default!)
    {
        MetaEntries = new [] { new PositionedMetaEntry<T>(0, text, metaData) };
    }

    protected MetaString(IEnumerable<MetaEntry<T>> entries) : this(entries.Positioned()) { }

    protected MetaString(IEnumerable<PositionedMetaEntry<T>> entries)
    {
        MetaEntries = entries.ToArray();
        CheckPositions();
    }

    public object Clone()
        => new MetaString<T>(MetaEntries);

    public MetaChar<T> this[Index index] => CharAt(index);

    public virtual MetaString<T> this[Range range] => GetRange(range);
    
    public MetaChar<T> CharAt(Index index)
    {
        var (entryIdx, charInx) = GetIndexes(index);
        var entry = MetaEntries[entryIdx];
        return new (entry.Text[charInx], entry.Data);
    }

    public override string ToString()
        => string.Join(null, MetaEntries.Select(e => $"[{e.Text}:{e.Data?.ToString()}]"));

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
    
    private string DebuggerDisplay => string.Join(null, MetaEntries.Select(e => $"[{e.Text}:{e.Data}]"));
}
