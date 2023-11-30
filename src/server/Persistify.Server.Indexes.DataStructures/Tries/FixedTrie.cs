using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public class FixedTrie<TIndexItem, TSearchItem, TItem> : IFixedTrie<TIndexItem, TSearchItem, TItem>
    where TIndexItem : IndexFixedTrieItem<TItem>
    where TSearchItem : SearchFixedTrieItem
{
    private readonly int _alphabetSize;
    private readonly FixedTrieNode<TItem> _root;

    public FixedTrie(
        int alphabetSize
    )
    {
        _alphabetSize = alphabetSize;
        _root = new FixedTrieNode<TItem>(alphabetSize);
        Depth = 0;
    }

    public int Depth { get; private set; }

    public void Insert(
        TIndexItem item
    )
    {
        if (item.Length > Depth)
        {
            Depth = item.Length;
        }

        var node = _root;
        for (var i = 0; i < item.Length; i++)
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

    public IEnumerable<TItem> Search(
        TSearchItem item
    )
    {
        var length = item.Length;

        var repeatedAnyIndexCount = 0;

        for (var i = 0; i < length; i++)
        {
            if (item.GetIndex(i) == item.RepeatedAnyIndex)
            {
                repeatedAnyIndexCount++;
            }
        }

        if (repeatedAnyIndexCount == length)
        {
            foreach (var result in _root.GetItems())
            {
                yield return result;
            }

            yield break;
        }

        length -= repeatedAnyIndexCount;

        var queue = new Queue<(FixedTrieNode<TItem> node, int searchLenght, int nodeDepth)>();
        queue.Enqueue((_root, 0, 0));

        while (queue.Count > 0)
        {
            var (node, searchLenght, nodeDepth) = queue.Dequeue();
            var index = item.GetIndex(nodeDepth);
            if (index == item.RepeatedAnyIndex)
            {
                queue.Enqueue((node, searchLenght, nodeDepth + 1));

                foreach (var child in node.GetChildren())
                {
                    if (child != null)
                    {
                        queue.Enqueue((child, searchLenght, nodeDepth));
                    }
                }
            }
            else if (searchLenght == length)
            {
                foreach (var result in node.GetItems())
                {
                    yield return result;
                }
            }
            else if (index == item.AnyIndex)
            {
                foreach (var child in node.GetChildren())
                {
                    if (child != null)
                    {
                        queue.Enqueue((child, searchLenght + 1, nodeDepth + 1));
                    }
                }
            }
            else
            {
                var child = node.GetChild(index);
                if (child != null)
                {
                    queue.Enqueue((child, searchLenght + 1, nodeDepth + 1));
                }
            }
        }
    }

    public int UpdateIf(
        Predicate<TItem> predicate,
        Action<TItem> action
    )
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
