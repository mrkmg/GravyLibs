namespace Gravy.MetaString;

public static class Extensions
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
    
    public static IEnumerable<MetaEntry<T>> WithoutPosition<T>(this IEnumerable<PositionedMetaEntry<T>> entries)
        => entries.Select(x => x.WithoutPosition());

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

    public static PositionedMetaEntry<T> PositionedAt<T>(this PositionedMetaEntry<T> entity, int offset) => new(offset, entity.Text, entity.Data);

    public static MetaString<T> Meta<T>(this string str, T metaData = default!) => new(str, metaData);
}