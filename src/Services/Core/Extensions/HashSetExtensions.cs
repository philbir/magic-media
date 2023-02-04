using System.Collections.Generic;

namespace MagicMedia.Extensions;

public static class HashSetExtensions
{
    public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            hashSet.Add(item);
        }
    }
}
