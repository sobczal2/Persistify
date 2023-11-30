using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public class FixedTrieNode<TItem>
{
    private readonly FixedTrieNode<TItem>?[] _children;

    public FixedTrieNode(
        int alphabetSize
    )
    {
        _children = new FixedTrieNode<TItem>?[alphabetSize];
    }

    public IndexFixedTrieItem<TItem>? Item { get; private set; }

    public void Insert(
        IndexFixedTrieItem<TItem> item
    )
    {
        if (Item is null)
        {
            Item = item;
        }
        else
        {
            Item.Merge(item.Value);
        }
    }

    public void Update(
        Action<TItem> action
    )
    {
        if (Item is null || Item.IsEmpty)
        {
            throw new InvalidOperationException("Item is null");
        }

        action(Item.Value);
    }

    public FixedTrieNode<TItem>? GetChild(
        int index
    )
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

    public void SetChild(
        int index,
        FixedTrieNode<TItem>? node
    )
    {
        _children[index] = node;
    }

    public void SetItemEmpty()
    {
        Item = null;
    }

    public IEnumerable<TItem> GetItems()
    {
        if (Item is not null)
        {
            yield return Item.Value;
        }

        foreach (var child in _children)
        {
            if (child is not null)
            {
                foreach (var item in child.GetItems())
                {
                    yield return item;
                }
            }
        }
    }
}
