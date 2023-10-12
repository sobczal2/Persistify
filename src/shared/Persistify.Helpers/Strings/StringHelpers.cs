using System.Collections.Generic;

namespace Persistify.Helpers.Strings;

public static class StringHelpers
{
    public static IEnumerable<string> GetNotEmptySuffixes(string value)
    {
        for (var i = value.Length - 1; i >= 0; i--)
        {
            yield return value[i..];
        }
    }
}
