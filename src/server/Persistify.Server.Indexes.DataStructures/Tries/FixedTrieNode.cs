using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public class FixedTrieNode<TItem>
where TItem : IComparable<TItem>
{
    private readonly FixedTrieNode<TItem>?[] _children;
    private readonly SortedSet<TItem> _items;

    public FixedTrieNode(int alphabetSize)
    {
        _children = new FixedTrieNode<TItem>?[alphabetSize];
        _items = new SortedSet<TItem>();
    }

    public void Insert(TItem item)
    {
        _items.Add(item);
    }

    public int Remove(Predicate<TItem> predicate)
    {
        var removedCount = 0;
        foreach (var item in _items)
        {
            if (predicate(item))
            {
                _items.Remove(item);
                removedCount++;
            }
        }
        return removedCount;
    }

    public IEnumerable<TItem> GetItems()
    {
        foreach (var item in _items)
        {
            yield return item;
        }
    }

    public IEnumerable<TItem> GetAllItems()
    {
        foreach (var item in _items)
        {
            yield return item;
        }
        foreach (var child in _children)
        {
            if (child != null)
            {
                foreach (var item in child.GetAllItems())
                {
                    yield return item;
                }
            }
        }
    }

    public FixedTrieNode<TItem>? GetChild(int index)
    {
        return _children[index];
    }

    public IEnumerable<FixedTrieNode<TItem>?> GetChildren()
    {
        foreach (var child in _children)
        {
            yield return child;
        }
    }

    public void SetChild(int index, FixedTrieNode<TItem>? node)
    {
        _children[index] = node;
    }
}
