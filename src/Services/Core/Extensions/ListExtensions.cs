using System.Collections.Generic;
using System.Linq;

namespace MagicMedia.Extensions;

public static class ListExtensions
{
    public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    public static void AddRangeIfNotNull<T>(this IList<T> source, IEnumerable<T>? items)
    {
        if (items != null)
        {
            foreach (T item in items)
            {
                source.Add(item);
            }
        }
    }
}
