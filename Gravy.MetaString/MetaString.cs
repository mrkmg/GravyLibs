﻿using System.Diagnostics;

namespace Gravy.MetaString;

public interface IMetaDebuggable<T>
{
    string DebugString(MetaString<T> metaString);
}

[DebuggerDisplay("{DebuggerDisplay}")]
public class MetaString<T>
{
    private PositionedMetaEntry<T>[] MetaEntries { get; }
    
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

    public MetaString(IEnumerable<MetaEntry<T>> entries) : this(entries.Positioned()) { }

    private MetaString(IEnumerable<PositionedMetaEntry<T>> entries)
    {
        MetaEntries = entries.ToArray();
        CheckPositions();
    }

    public object Clone()
        => new MetaString<T>(MetaEntries);

    public MetaChar<T> this[Index index] => CharAt(index);

    public MetaString<T> this[Range range] => GetRange(range);
    
    public MetaChar<T> CharAt(Index index)
    {
        var (entryIdx, charInx) = GetIndexes(index);
        var entry = MetaEntries[entryIdx];
        return new (entry.Text[charInx], entry.Data);
    }

    public override string ToString() => RawText;
    
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

    #region Operators

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
        => new(str, default!);

    #endregion

    #region StringMethods

    public static MetaString<T> Empty { get; } = new(Array.Empty<PositionedMetaEntry<T>>());
    public int Length => MetaEntries.Sum(me => me.Text.Length);

    public MetaString<T> PadLeft(int length)
        => PadLeft(' ', length);

    public MetaString<T> PadLeft(char ch, int length)
    {
        var len = Length;
        if (len >= length)
            return this;
        return new string(ch, length - len) + this;
    }

    public MetaString<T> PadRight(int length)
        => PadRight(' ', length);

    public MetaString<T> PadRight(char ch, int length)
    {
        var len = Length;
        if (len >= length)
            return this;
        return this + new string(ch, length - len);
    }
    
    public MetaString<T> Trim()
        => TrimInternal(this, s => s.Trim(), s => s.TrimStart(), s => s.TrimEnd());

    public MetaString<T> Trim(char ch)
        => TrimInternal(this, s => s.Trim(ch), s => s.TrimStart(ch), s => s.TrimEnd(ch));
    
    public MetaString<T> Trim(params char[] ch)
        => TrimInternal(this, s => s.Trim(ch), s => s.TrimStart(ch), s => s.TrimEnd(ch));

    public MetaString<T> Substring(int start, int length = 0)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "length must be >= 0");
        return GetRange(new(start, length == 0 ? new Index(0, true) : new(start + length - 1)));
    }

    public bool Contains(string str)
    {
        if (str.Length == 0)
            return true;
        var strIndex = 0;
        return MetaEntries
            .SelectMany(entry => entry.Text)
            .Any(ch => ch == str[strIndex] && ++strIndex == str.Length);
    }

    public bool Contains(MetaString<T> other)
    {
        if (other.MetaEntries.Length == 0)
            return true;
        var myIndex = 0;
        var otherIndex = 0;
        while (myIndex < MetaEntries.Length)
        {
            if (MetaEntries[myIndex].DataEquals(other.MetaEntries[otherIndex]))
            {
                if (otherIndex == 0 &&
                    MetaEntries[myIndex].Text.EndsWith(other.MetaEntries[otherIndex].Text))
                    otherIndex++;
                else if (otherIndex == other.MetaEntries.Length - 1 &&
                         MetaEntries[myIndex].Text.StartsWith(other.MetaEntries[otherIndex].Text))
                    otherIndex++;
                else if (MetaEntries[myIndex].Text == other.MetaEntries[otherIndex].Text)
                    otherIndex++;
                else
                    otherIndex = 0;
            }
            else
            {
                otherIndex = 0;
            }
            myIndex++;
            if (otherIndex == other.MetaEntries.Length)
                return true;
        }
        return false;
    }

    private static Range? IndexToRange(int index, int length = 1)
    {
        if (index == -1) return null;
        return new Range(index, index + length);
    }

    public MetaString<T>[] Split(char separator)
        => Split(separator, int.MaxValue);
    public MetaString<T>[] Split(char separator, StringSplitOptions options)
        => Split(separator, int.MaxValue, options);
    public MetaString<T>[] Split(char separator, int count, StringSplitOptions options = StringSplitOptions.None)
        => SplitInternal(s => IndexToRange(s.IndexOf(separator)), count, options);

    public MetaString<T>[] Split(char[] separators)
        => Split(separators, int.MaxValue);
    public MetaString<T>[] Split(char[] separator, StringSplitOptions options)
        => Split(separator, int.MaxValue, options);
    public MetaString<T>[] Split(char[] separator, int count, StringSplitOptions options = StringSplitOptions.None)
        => SplitInternal(s => IndexToRange(s.IndexOfAny(separator)), count, options);

    public MetaString<T>[] Split(string separator)
        => Split(separator, int.MaxValue);
    public MetaString<T>[] Split(string separator, StringSplitOptions options)
        => Split(separator, int.MaxValue, options);
    public MetaString<T>[] Split(string separator, int count, StringSplitOptions options = StringSplitOptions.None)
        => SplitInternal(s => IndexToRange(s.IndexOf(separator, StringComparison.Ordinal), separator.Length), count, options);
    
    public MetaString<T>[] Split(string[] separator)
        => Split(separator, int.MaxValue);
    public MetaString<T>[] Split(string[] separator, StringSplitOptions options)
        => Split(separator, int.MaxValue, options);
    public MetaString<T>[] Split(string[] separator, int count, StringSplitOptions options = StringSplitOptions.None)
        => SplitInternal(s =>
        {
            var min = int.MaxValue;
            var minL = 0;
            foreach (var str in separator)
            {
                var index = s.IndexOf(str, StringComparison.Ordinal);
                if (index == -1 || index >= min) continue;
                min = index;
                minL = str.Length;
            }
            if (min == int.MaxValue)
                return null;
            return new Range(min, minL);
        }, count, options);
    
    private static MetaString<T> TrimInternal(MetaString<T> cs, Func<string, string> trim, Func<string, string> trimStart, Func<string, string> trimEnd)
    {
        if (cs.MetaEntries.Length == 0)
            return Empty;

        var sourceEntries = cs.MetaEntries;
        var targetEntries = new List<MetaEntry<T>>();
        var i = 0;
        while (i < sourceEntries.Length)
        {
            MetaEntry<T> newMetaEntry;
            
            if (i == sourceEntries.Length - 1)
            {
                newMetaEntry = new(trim(sourceEntries[i].Text), sourceEntries[i].Data);
                return newMetaEntry.Text.Length == 0 ? Empty : new(new[] { newMetaEntry });
            }

            newMetaEntry = new(trimStart(sourceEntries[i].Text), sourceEntries[i].Data);
            if (newMetaEntry.Text.Length != 0)
            {
                targetEntries.Add(newMetaEntry);
                break;
            }
            i++;
        }
        
        var j = sourceEntries.Length - 1;
        while (true)
        {
            var newMetaEntry = new MetaEntry<T>(trimEnd(sourceEntries[j].Text), sourceEntries[j].Data);
            
            if (newMetaEntry.Text.Length > 0)
            {
                while (++i < j)
                {
                    var entry = new MetaEntry<T>(sourceEntries[i].Text, sourceEntries[i].Data);
                    targetEntries.Add(entry);
                }
                targetEntries.Add(newMetaEntry);
                break;
            }

            if (--j > i)
                continue;
            targetEntries[^1] = new(trimEnd(targetEntries[^1].Text), targetEntries[^1].Data);
            break;
        }
        return new(targetEntries);
    }
    
    private MetaString<T>[] SplitInternal(Func<string, Range?> nextSeparator, int count, StringSplitOptions options)
    {
        var newEntries = new List<MetaEntry<T>>();
        var result = new List<MetaString<T>>();
        
        void AddResult()
        {
            var metaString = new MetaString<T>(newEntries);
            newEntries.Clear();
            
            if (options.HasFlag(StringSplitOptions.TrimEntries))
                metaString = metaString.Trim();
            
            if (metaString.Length == 0 && options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
                return;
            
            if (result.Count < count)
                result.Add(metaString);
            else
                result[^1] += metaString;
        }
        
        foreach (var entry in MetaEntries)
        {
            if (entry.Text.Length == 0)
                continue;

            var remaining = entry.Text;

            while (remaining != string.Empty)
            {
                var next = nextSeparator(remaining);
                
                if (next is null)
                {
                    newEntries.Add(new(remaining, entry.Data));
                    break;
                }
                
                if (next.Value.Start.Value == 0)
                {
                    AddResult();
                    remaining = remaining[next.Value.End..];
                    continue;
                }
                
                newEntries.Add(new(remaining[..next.Value.End], entry.Data));
                remaining = remaining[next.Value.End..];
            }
        }
        if (newEntries.Count > 0) 
            AddResult();
        
        return result.ToArray();
    }

    #endregion

    #region Privates
    
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
    
    private string DebuggerDisplay => 
        MetaEntries.Length > 0 && MetaEntries[0].Data is IMetaDebuggable<T> metaDebuggable 
            ? metaDebuggable.DebugString(this) 
            : string.Join(null, MetaEntries.Select(e => $"[{e.Text}:{e.Data}]"));

    
    #endregion
}
