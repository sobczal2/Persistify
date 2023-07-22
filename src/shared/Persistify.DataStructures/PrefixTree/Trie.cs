using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Persistify.DataStructures.PrefixTree;

public class Trie<TValue> : ITrie<TValue>
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly int[] AllIndexes = Enumerable.Range(0, 62).ToArray();
    private readonly ReaderWriterLockSlim _lock;

    private readonly TrieNode<TValue> _root;

    public Trie()
    {
        _root = new TrieNode<TValue>();
        _lock = new ReaderWriterLockSlim();
    }

    public void Add(string key, TValue value)
    {
        Add(key, value, true);
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

    private void Add(string key, TValue value, bool useLock)
    {
        var indexes = GetIndexes(key, true);

        if (useLock)
        {
            _lock.EnterWriteLock();
        }

        try
        {
            Add(_root, value, indexes);
        }
        finally
        {
            if (useLock)
            {
                _lock.ExitWriteLock();
            }
        }
    }

    private static void Add(TrieNode<TValue> node, TValue value, ReadOnlySpan<int[]> indexes)
    {
        if (indexes.Length == 0)
        {
            node.Values.Add(value);
            return;
        }

        var index = indexes[0];
        for (var i = 0; i < index.Length; i++)
        {
            Add(node.Children[index[i]] ??= new TrieNode<TValue>(), value, indexes[1..]);
        }
    }

    private void Search(TrieNode<TValue> node, ICollection<TValue> values, ReadOnlySpan<int[]> indexes,
        bool exact)
    {
        if (indexes.Length == 0)
        {
            if (exact)
            {
                if (node.Values.Count <= 0)
                {
                    return;
                }

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

    private static void CollectAllValues(TrieNode<TValue> node, ICollection<TValue> values)
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

    private static void Remove(TrieNode<TValue> node, Predicate<TValue> predicate)
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
