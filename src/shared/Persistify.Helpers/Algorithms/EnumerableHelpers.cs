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
        if (enumerables.Length == 0)
        {
            yield break;
        }

        var enumerators = GetEnumerators(enumerables);

        try
        {
            var canAdvance = true;

            foreach (var enumerator in enumerators)
            {
                if (!enumerator.MoveNext())
                {
                    canAdvance = false;
                    break;
                }
            }

            if (!canAdvance)
            {
                yield break;
            }

            while (true)
            {
                var firstValue = enumerators[0].Current;
                var areEqual = true;
                for (var i = 1; i < enumerators.Length; i++)
                {
                    if (comparer.Compare(enumerators[i].Current, firstValue) != 0)
                    {
                        areEqual = false;
                        break;
                    }
                }

                if (areEqual)
                {
                    yield return firstValue;
                    canAdvance = true;
                    foreach (var enumerator in enumerators)
                    {
                        if (!enumerator.MoveNext())
                        {
                            canAdvance = false;
                            break;
                        }
                    }

                    if (!canAdvance)
                    {
                        break;
                    }

                    continue;
                }

                var minValue = enumerators[0].Current;

                for (var i = 1; i < enumerators.Length; i++)
                {
                    if (comparer.Compare(enumerators[i].Current, minValue) < 0)
                    {
                        minValue = enumerators[i].Current;
                    }
                }

                canAdvance = true;
                foreach (var enumerator in enumerators)
                {
                    while (comparer.Compare(enumerator.Current, minValue) <= 0)
                    {
                        if (!enumerator.MoveNext())
                        {
                            canAdvance = false;
                            break;
                        }
                    }

                    if (!canAdvance)
                    {
                        break;
                    }
                }

                if (!canAdvance)
                {
                    break;
                }
            }
        }
        finally
        {
            foreach (var enumerator in enumerators)
            {
                enumerator.Dispose();
            }
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
