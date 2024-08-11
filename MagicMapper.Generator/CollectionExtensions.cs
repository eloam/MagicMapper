using System.Collections.Generic;

namespace MagicMapper.Generator;

internal static class CollectionExtensions
{
    public static void AddIfNotNull<T>(this ICollection<T> source, T? item)
    {
        if (item != null)
            source.Add(item);
    }
}