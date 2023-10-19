using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public class FixedTrie<TItem> : IFixedTrie<TItem>
{
    private readonly int _alphabetSize;
    private readonly FixedTrieNode<IndexFixedTrieItem<TItem>> _root;

    public FixedTrie(int alphabetSize)
    {
        _alphabetSize = alphabetSize;
        _root = new FixedTrieNode<IndexFixedTrieItem<TItem>>(alphabetSize);
    }

    public void Insert(IndexFixedTrieItem<TItem> item)
    {
        var node = _root;
        for(var i = 0; i < item.Length; i++)
        {
            var index = item.GetIndex(i);
            var child = node.GetChild(index);
            if (child == null)
            {
                child = new FixedTrieNode<IndexFixedTrieItem<TItem>>(_alphabetSize);
                node.SetChild(index, child);
            }

            node = child;
        }

        node.Insert(item);
    }

    public IEnumerable<(TItem Item, int Distance)> Search(SearchFixedTrieItem item)
    {
        var node = _root;
        var distance = 0;

        for (var i = 0; i < item.Length; i++)
        {
            var index = item.GetIndex(i);
            var child = node.GetChild(index);
            if (child == null)
            {
                yield break;
            }

            node = child;
            distance++;
        }

        foreach (var child in node.GetAllItems())
        {
            yield return (child.Value, distance);
        }
    }

    public int Remove(Predicate<TItem> predicate)
    {
        return Remove(_root, predicate);
    }

    private int Remove(FixedTrieNode<IndexFixedTrieItem<TItem>> node, Predicate<TItem> predicate)
    {
        var removedCount = 0;
        removedCount += node.Remove(x => predicate(x.Value));
        for (var i = 0; i < _alphabetSize; i++)
        {
            var child = node.GetChild(i);
            if (child != null)
            {
                removedCount += Remove(child, predicate);
            }
        }

        return removedCount;
    }
}
