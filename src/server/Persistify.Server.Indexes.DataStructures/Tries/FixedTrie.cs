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

    public IEnumerable<TItem> Search(TSearchItem item)
    {
        var results = new List<TItem>();
        Search(_root, item, 0, results);
        return results;
    }

    private void Search(FixedTrieNode<TItem> node, TSearchItem searchItem, int depth, List<TItem> results)
    {
        if (node == null)
        {
            return;
        }

        // If the current depth equals the search item's length, add the items in the current node.
        if (depth == searchItem.Length)
        {
            results.AddRange(node.GetItems());
            return;
        }

        var index = searchItem.GetIndex(depth);
        if (index == searchItem.RepeatedAnyIndex)
        {
            // If RepeatedAnyIndex, recurse for all children.
            foreach (var child in node.GetChildren())
            {
                Search(child!, searchItem, depth + 1, results);
            }
            // Also, continue with the same node, as RepeatedAnyIndex can match multiple characters.
            Search(node, searchItem, depth + 1, results);
        }
        else if (index == searchItem.AnyIndex)
        {
            // If AnyIndex, recurse for all children.
            foreach (var child in node.GetChildren())
            {
                Search(child!, searchItem, depth + 1, results);
            }
        }
        else
        {
            // Regular case, follow the specific index.
            var child = node.GetChild(index);
            if (child != null)
            {
                Search(child, searchItem, depth + 1, results);
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
