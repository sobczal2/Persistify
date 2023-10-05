using System;
using System.Collections.Generic;

namespace Persistify.Helpers.Algorithms;

public static class EnumerableHelpers
{
    public static IEnumerable<T> MergeSorted<T>(IComparer<T> comparer, params IEnumerable<T>[] enumerables)
    {
        var enumerators = GetEnumerators(enumerables);
        var heap = new SortedSet<(T Value, int Index)>(
            Comparer<(T Value, int Index)>.Create((x, y) => comparer.Compare(x.Value, y.Value)));
        for (var i = 0; i < enumerators.Length; i++)
        {
            if (enumerators[i].MoveNext())
            {
                heap.Add((enumerators[i].Current, i));
            }
        }

        while (heap.Count > 0)
        {
            var min = heap.Min;
            heap.Remove(min);
            yield return min.Value;
            if (enumerators[min.Index].MoveNext())
            {
                heap.Add((enumerators[min.Index].Current, min.Index));
            }
        }

        foreach (var enumerator in enumerators)
        {
            enumerator.Dispose();
        }
    }

    public static IEnumerable<T> IntersectSorted<T>(IComparer<T> comparer, params IEnumerable<T>[] enumerables)
    {
        var enumerators = GetEnumerators(enumerables);

        while (true)
        {
            var canAdvance = true;
            foreach (var enumerator in enumerators)
            {
                canAdvance &= enumerator.MoveNext();
            }

            if (!canAdvance)
            {
                break;
            }

            var firstValue = enumerators[0].Current;
            if (Array.TrueForAll(enumerators, it => comparer.Compare(it.Current, firstValue) == 0))
            {
                yield return firstValue;
                continue;
            }

            var minValue = firstValue;
            foreach (var iterator in enumerators)
            {
                if (comparer.Compare(iterator.Current, minValue) < 0)
                {
                    minValue = iterator.Current;
                }
            }

            foreach (var iterator in enumerators)
            {
                while (comparer.Compare(iterator.Current, minValue) == 0)
                {
                    if (!iterator.MoveNext())
                    {
                        break;
                    }
                }
            }
        }

        foreach (var iterator in enumerators)
        {
            iterator.Dispose();
        }
    }

    public static IEnumerator<T>[] GetEnumerators<T>(IEnumerable<T>[] enumerables)
    {
        var enumerators = new IEnumerator<T>[enumerables.Length];
        for (var i = 0; i < enumerables.Length; i++)
        {
            enumerators[i] = enumerables[i].GetEnumerator();
        }

        return enumerators;
    }
}
