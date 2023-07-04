namespace Persistify.DataStructures.PrefixTree;

public class PrefixTree<TValue> : IPrefixTree<TValue>
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly int[] AllIndexes = Enumerable.Range(0, 62).ToArray();

    private readonly PrefixTreeNode<TValue> _root;
    private readonly ReaderWriterLockSlim _lock;

    public PrefixTree()
    {
        _root = new PrefixTreeNode<TValue>();
        _lock = new ReaderWriterLockSlim();
    }

    public void Add(string key, TValue value)
    {
        Add(key, value, true);
    }

    private void Add(string key, TValue value, bool useLock)
    {
        var indexes = GetIndexes(key, true);

        if (useLock) _lock.EnterWriteLock();
        try
        {
            Add(_root, value, indexes);
        }
        finally
        {
            if (useLock) _lock.ExitWriteLock();
        }
    }

    private static void Add(PrefixTreeNode<TValue> node, TValue value, ReadOnlySpan<int[]> indexes)
    {
        if (indexes.Length == 0)
        {
            node.Values.Add(value);
            return;
        }

        var index = indexes[0];
        for (var i = 0; i < index.Length; i++)
            Add(node.Children[index[i]] ??= new PrefixTreeNode<TValue>(), value, indexes[1..]);
    }

    public void AddRange(IEnumerable<KeyValuePair<string, TValue>> items)
    {
        _lock.EnterWriteLock();
        try
        {
            foreach (var (key, value) in items)
            {
                Add(key, value, false);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public ICollection<TValue> Search(string key, bool caseSensitive, bool exact)
    {
        var indexes = GetIndexes(key, caseSensitive);
        var values = new List<TValue>();

        _lock.EnterReadLock();

        try
        {
            Search(_root, values, indexes, exact);
        }
        finally
        {
            _lock.ExitReadLock();
        }

        return values;
    }

    private void Search(PrefixTreeNode<TValue> node, ICollection<TValue> values, ReadOnlySpan<int[]> indexes, bool exact)
    {
        if (indexes.Length == 0)
        {
            if (exact)
            {
                if (node.Values.Count <= 0)
                    return;

                foreach (var value in node.Values)
                {
                    values.Add(value);
                }
            }
            else
            {
                CollectAllValues(node, values);
            }

            return;
        }

        var index = indexes[0];
        for (var i = 0; i < index.Length; i++)
        {
            var child = node.Children[index[i]];
            if (child != null)
            {
                Search(child, values, indexes[1..], exact);
            }
        }
    }

    private static void CollectAllValues(PrefixTreeNode<TValue> node, ICollection<TValue> values)
    {
        foreach (var value in node.Values)
        {
            values.Add(value);
        }

        for (var i = 0; i < node.Children.Length; i++)
        {
            var child = node.Children[i];
            if (child != null)
            {
                CollectAllValues(child, values);
            }
        }
    }

    public void Remove(Predicate<TValue> predicate)
    {
        _lock.EnterWriteLock();
        try
        {
            Remove(_root, predicate);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private static void Remove(PrefixTreeNode<TValue> node, Predicate<TValue> predicate)
    {
        for (var i = 0; i < node.Children.Length; i++)
        {
            var child = node.Children[i];

            if (child == null)
            {
                continue;
            }

            Remove(child, predicate);
            
            if (child.Values.Count <= 0 && child.Children.All(c => c == null))
            {
                node.Children[i] = null;
            }
        }

        node.Values.RemoveWhere(predicate);
    }

    private static int[][] GetIndexes(ReadOnlySpan<char> valueSpan, bool caseSensitive)
    {
        var indexes = new int[valueSpan.Length][];

        for (var i = 0; i < valueSpan.Length; i++)
        {
            indexes[i] = valueSpan[i] switch
            {
                '?' => AllIndexes,
                var c and >= '0' and <= '9' => new[] { c - '0' },
                var c and >= 'a' and <= 'z' => caseSensitive
                    ? new[] { c - 'a' + 10 }
                    : new[] { c - 'a' + 10, c - 'a' + 36 },
                var c and >= 'A' and <= 'Z' => caseSensitive
                    ? new[] { c - 'A' + 36 }
                    : new[] { c - 'A' + 10, c - 'A' + 36 },
                _ => throw new ArgumentException($"Invalid character '{valueSpan[i]}' at position {i}.")
            };
        }

        return indexes;
    }
}
