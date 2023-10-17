using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public class FixedTrie<TItem> : ITrie<TItem>
    where TItem : IComparable<TItem>, IEnumerable<int>
{
    private readonly int _alphabetSize;
    private readonly FixedTrieNode<TItem> _root;

    public FixedTrie(int alphabetSize)
    {
        _alphabetSize = alphabetSize;
        _root = new FixedTrieNode<TItem>(alphabetSize);
    }

    public void Insert(TItem item)
    {
        var node = _root;
        foreach (var index in item)
        {
            var child = node.GetChild(index);
            if (child == null)
            {
                child = new FixedTrieNode<TItem>(_alphabetSize);
                node.SetChild(index, child);
            }

            node = child;
        }

        node.Insert(item);
    }

    public IEnumerable<(TItem Item, int Distance)> Search(IEnumerable<int> item)
    {
        var node = _root;
        var distance = 0;

        foreach (var index in item)
        {
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
            yield return (child, distance);
        }
    }

    public int Remove(Predicate<TItem> predicate)
    {
        return Remove(_root, predicate);
    }

    private int Remove(FixedTrieNode<TItem> node, Predicate<TItem> predicate)
    {
        var removedCount = 0;
        removedCount += node.Remove(predicate);
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
