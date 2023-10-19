﻿using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public class FixedTrie<TIndexItem, TSearchItem, TItem> : IFixedTrie<TIndexItem, TSearchItem, TItem>
    where TIndexItem : IndexFixedTrieItem<TItem>
    where TSearchItem : SearchFixedTrieItem
{
    private readonly int _alphabetSize;
    private readonly FixedTrieNode<TItem> _root;

    public FixedTrie(int alphabetSize)
    {
        _alphabetSize = alphabetSize;
        _root = new FixedTrieNode<TItem>(alphabetSize);
    }

    public void Insert(TIndexItem item)
    {
        var node = _root;
        for(var i = 0; i < item.Length; i++)
        {
            var index = item.GetIndex(i);
            var child = node.GetChild(index);
            if (child is null)
            {
                child = new FixedTrieNode<TItem>(_alphabetSize);
                node.SetChild(index, child);
            }

            node = child;
        }

        node.Insert(item);
    }

    public IEnumerable<TItem> Search(TSearchItem item)
    {
        var node = _root;

        for (var i = 0; i < item.Length; i++)
        {
            var index = item.GetIndex(i);
            var child = node.GetChild(index);
            if (child is null)
            {
                yield break;
            }

            node = child;
        }

        if (node.Item != null)
        {
            yield return node.Item.Value;
        }
    }

    public int UpdateIf(Predicate<TItem> predicate, Action<TItem> action)
    {
        var count = 0;
        var queue = new Queue<FixedTrieNode<TItem>>();
        queue.Enqueue(_root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node.Item != null && predicate(node.Item.Value))
            {
                node.Update(action);
                if (node.Item.IsEmpty)
                {
                    node.SetItemEmpty();
                }
                count++;
            }

            foreach (var child in node.GetChildren())
            {
                if (child != null)
                {
                    queue.Enqueue(child);
                }
            }
        }

        RemoveDeadEnds();
        return count;
    }

    private void RemoveDeadEnds()
    {
        var queue = new Queue<FixedTrieNode<TItem>>();
        queue.Enqueue(_root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node.Item != null && node.Item.IsEmpty)
            {
                node.SetItemEmpty();
            }

            foreach (var child in node.GetChildren())
            {
                if (child != null)
                {
                    queue.Enqueue(child);
                }
            }
        }
    }
}
