using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Persistify.DataStructures.Tries;

public class TrieNode<TItem>
{
    private static readonly int[] _allIndexes = Enumerable.Range(0, 62).ToArray();
    [JsonProperty] private readonly TrieNode<TItem>?[] _children;
    [JsonProperty] private readonly List<TItem> _items;

    public TrieNode()
    {
        _children = new TrieNode<TItem>[62];
        _items = new List<TItem>();
    }

    public void Add(ReadOnlySpan<char> key, TItem item)
    {
        if (key.IsEmpty)
        {
            _items.Add(item);
            return;
        }

        var index = GetIndex(key[0]);
        _children[index] ??= new TrieNode<TItem>();
        _children[index]!.Add(key[1..], item);
    }

    private List<TItem> GetAllItems()
    {
        var items = new List<TItem>(_items);
        foreach (var child in _children)
        {
            if (child is null)
                continue;
            items.AddRange(child.GetAllItems());
        }

        return items;
    }

    public List<TItem> Search(ReadOnlySpan<char> key, bool caseSensitive, bool exact)
    {
        if (key.IsEmpty) return exact ? new List<TItem>(_items) : GetAllItems();

        var indexes = GetIndexes(key[0], caseSensitive);
        var items = new List<TItem>();
        foreach (var index in indexes)
        {
            if (_children[index] is null)
                continue;
            items.AddRange(_children[index]!.Search(key[1..], caseSensitive, exact));
        }

        return items;
    }


    public int Remove(Predicate<TItem> predicate)
    {
        var deletedCount = _items.RemoveAll(predicate);
        foreach (var child in _children)
        {
            if (child is null)
                continue;
            deletedCount += child.Remove(predicate);
        }

        return deletedCount;
    }

    private static int[] GetIndexes(char c, bool caseSensitive)
    {
        if (c >= '0' && c <= '9') return new[] { c - '0' };

        if (c >= 'a' && c <= 'z') return caseSensitive ? new[] { c - 'a' + 10 } : new[] { c - 'a' + 10, c - 'a' + 36 };

        if (c >= 'A' && c <= 'Z') return caseSensitive ? new[] { c - 'A' + 36 } : new[] { c - 'A' + 36, c - 'A' + 10 };

        if (c == '?') return _allIndexes;

        throw new ArgumentException($"Invalid character: {c}");
    }

    private static int GetIndex(char c)
    {
        if (c >= '0' && c <= '9') return c - '0';

        if (c >= 'a' && c <= 'z') return c - 'a' + 10;

        if (c >= 'A' && c <= 'Z') return c - 'A' + 36;

        throw new ArgumentException($"Invalid character: {c}");
    }
}