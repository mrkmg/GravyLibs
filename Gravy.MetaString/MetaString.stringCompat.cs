namespace Gravy.MetaString;

public partial class MetaString<T>
{
    public static MetaString<T> Empty { get; } = new(Array.Empty<PositionedMetaEntry<T>>());
    public int Length => MetaEntries.Sum(me => me.Text.Length);

    public virtual MetaString<T> PadLeft(int length)
        => PadLeft(' ', length);

    public virtual MetaString<T> PadLeft(char ch, int length)
    {
        var len = Length;
        if (len >= length)
            return this;
        return new string(ch, length - len) + this;
    }

    public virtual MetaString<T> PadRight(int length)
        => PadRight(' ', length);

    public virtual MetaString<T> PadRight(char ch, int length)
    {
        var len = Length;
        if (len >= length)
            return this;
        return this + new string(ch, length - len);
    }
    
    public virtual MetaString<T> Trim()
        => TrimInternal(this, s => s.Trim(), s => s.TrimStart(), s => s.TrimEnd());

    public virtual MetaString<T> Trim(char ch)
        => TrimInternal(this, s => s.Trim(ch), s => s.TrimStart(ch), s => s.TrimEnd(ch));
    
    public virtual MetaString<T> Trim(params char[] ch)
        => TrimInternal(this, s => s.Trim(ch), s => s.TrimStart(ch), s => s.TrimEnd(ch));

    public virtual MetaString<T> Substring(int start, int length = 0)
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

    public virtual MetaString<T>[] Split(char separator)
        => Split(separator, int.MaxValue);
    public virtual MetaString<T>[] Split(char separator, StringSplitOptions options)
        => Split(separator, int.MaxValue, options);
    public virtual MetaString<T>[] Split(char separator, int count, StringSplitOptions options = StringSplitOptions.None)
        => SplitInternal(s => IndexToRange(s.IndexOf(separator)), count, options);

    public virtual MetaString<T>[] Split(char[] separators)
        => Split(separators, int.MaxValue);
    public virtual MetaString<T>[] Split(char[] separator, StringSplitOptions options)
        => Split(separator, int.MaxValue, options);
    public virtual MetaString<T>[] Split(char[] separator, int count, StringSplitOptions options = StringSplitOptions.None)
        => SplitInternal(s => IndexToRange(s.IndexOfAny(separator)), count, options);

    public virtual MetaString<T>[] Split(string separator)
        => Split(separator, int.MaxValue);
    public virtual MetaString<T>[] Split(string separator, StringSplitOptions options)
        => Split(separator, int.MaxValue, options);
    public virtual MetaString<T>[] Split(string separator, int count, StringSplitOptions options = StringSplitOptions.None)
        => SplitInternal(s => IndexToRange(s.IndexOf(separator, StringComparison.Ordinal), separator.Length), count, options);
    
    public virtual MetaString<T>[] Split(string[] separator)
        => Split(separator, int.MaxValue);
    public virtual MetaString<T>[] Split(string[] separator, StringSplitOptions options)
        => Split(separator, int.MaxValue, options);
    public virtual MetaString<T>[] Split(string[] separator, int count, StringSplitOptions options = StringSplitOptions.None)
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
}