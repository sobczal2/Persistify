using System.Collections.Generic;
using System.Linq;

namespace Persistify.Extensions;

public static class EnumerableExtensions
{
    public static List<T> ToListOptimized<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is List<T> list)
            return list;
        return enumerable.ToList();
    }
}