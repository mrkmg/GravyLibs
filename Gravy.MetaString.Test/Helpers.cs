namespace Gravy.MetaString.Test;

public static class Helpers
{
    public static IEnumerable<(int Offset, int Data)> OffsetInts(this MetaString<int> msInt)
        => msInt.MetaEntries.Select(e => (e.Offset, e.Data));
}