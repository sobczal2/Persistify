using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Persistify.DataStructures.Tries;

public class TrieNode<TItem>
{
    [JsonProperty] private readonly TrieNode<TItem>?[] _children;
    [JsonProperty] private readonly List<TItem> _items;

    public TrieNode()
    {
        _children = new TrieNode<TItem>[36];
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

    public List<TItem> Search(ReadOnlySpan<char> key)
    {
        if (key.IsEmpty)
            return GetAllItems();

        if (key[0] == '$')
        {
            var items = new List<TItem>();
            for (var i = 0; i < _children.Length; i++)
            {
                if (_children[i] is null)
                    continue;
                items.AddRange(_children[i]!.Search(key[1..]));
            }

            return items;
        }

        var index = GetIndex(key[0]);
        return _children[index] is null ? new List<TItem>() : _children[index]!.Search(key[1..]);
    }

    public int Remove(Predicate<TItem> predicate)
    {
        var deletedCount = _items.RemoveAll(predicate);
        for (var i = 0; i < _children.Length; i++)
        {
            if (_children[i] is null)
                continue;
            deletedCount += _children[i]!.Remove(predicate);
        }

        return deletedCount;
    }

    private static int GetIndex(char c)
    {
        return c switch
        {
            >= '0' and <= '9' => c - '0',
            >= 'a' and <= 'z' => c - 'a' + 10,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, "Invalid character")
        };
    }
}