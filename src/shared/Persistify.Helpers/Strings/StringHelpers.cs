using System;
using System.Runtime.CompilerServices;

namespace Persistify.Helpers.Strings;

public static class StringHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] GetSuffixes(string value)
    {
        var result = new string[value.Length];
        var tokenSpan = value.AsSpan();
        for (var j = 0; j < tokenSpan.Length; j++)
        {
            result[j] = tokenSpan[j..].ToString();
        }

        return result;
    }
}
