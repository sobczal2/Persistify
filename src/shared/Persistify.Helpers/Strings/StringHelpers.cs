using System.Collections.Generic;

namespace Persistify.Helpers.Strings;

public static class StringHelpers
{
    public static IEnumerable<string> GetSuffixes(string value)
    {
        for (var i = 0; i < value.Length; i++)
        {
            yield return value[i..];
        }
    }
}
