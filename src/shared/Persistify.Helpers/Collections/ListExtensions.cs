using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Helpers.Collections;

public static class ListExtensions
{
    public static List<TTo> ListSelect<TFrom, TTo>(this List<TFrom> list, Func<TFrom, TTo> selector)
    {
        var result = new List<TTo>(list.Count);

        foreach (var item in list)
        {
            result.Add(selector(item));
        }

        return result;
    }

    public static async ValueTask<List<TTo>> ListSelectAsync<TFrom, TTo>(this List<TFrom> list, Func<TFrom, ValueTask<TTo>> selector)
    {
        var result = new List<TTo>(list.Count);

        foreach (var item in list)
        {
            result.Add(await selector(item));
        }

        return result;
    }
}
