using System;
using System.Collections.Generic;

namespace Persistify.Helpers.Collections;

public static class EnumerableHelpers
{
    public static IEnumerable<T> MergeSorted<T>(IComparer<T> comparer, Func<T, T, T> mergeFunc,
        params IEnumerable<T>[] enumerables)
    {
        var enumerators = GetEnumerators(enumerables);
        var heap = new SortedSet<(T Value, int Index)>(Comparer<(T Value, int Index)>.Create((x, y) =>
        {
            var comp = comparer.Compare(x.Value, y.Value);
            return comp != 0 ? comp : x.Index.CompareTo(y.Index);
        }));

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

            var mergedValue = min.Value;

            var nextHeap = new SortedSet<(T Value, int Index)>(heap.Comparer);
            foreach (var item in heap)
            {
                if (comparer.Compare(item.Value, min.Value) == 0)
                {
                    mergedValue = mergeFunc(mergedValue, item.Value);
                }
                else
                {
                    nextHeap.Add(item);
                }

                if (enumerators[item.Index].MoveNext())
                {
                    nextHeap.Add((enumerators[item.Index].Current, item.Index));
                }
            }

            heap = nextHeap;

            yield return mergedValue;

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

    public static IEnumerable<T> IntersectSorted<T>(IComparer<T> comparer, Func<T, T, T> mergeFunc,
        params IEnumerable<T>[] enumerables)
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
                    var mergedValue = enumerators[0].Current;

                    for (var i = 1; i < enumerators.Length; i++)
                    {
                        mergedValue = mergeFunc(mergedValue, enumerators[i].Current);
                    }

                    yield return mergedValue;

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
