using System;
using System.Linq;

namespace Persistify.Extensions;

public static class RandomExtensions
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

    public static string NextString(this Random random, int length)
    {
        return new string(Enumerable.Repeat(Chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static TEnum NextEnum<TEnum>(this Random random) where TEnum : Enum
    {
        var values = Enum.GetValues(typeof(TEnum));
        return (TEnum)values.GetValue(random.Next(values.Length));
    }

    public static bool NextBool(this Random random)
    {
        return random.Next(2) == 0;
    }
}