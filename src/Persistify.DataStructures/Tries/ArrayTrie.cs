using System;
using System.Collections.Generic;
using System.Linq;
using Persistify.DataStructures.Tries.Abstractions;
using Persistify.DataStructures.Tries.Exceptions;

namespace Persistify.DataStructures.Tries;

public class ArrayTrie<TItem> : ITrie<TItem> where TItem : struct, IComparable<TItem>, IConvertible
{
    private ArrayTrieNode _root;

    public ArrayTrie()
    {
        _root = new ArrayTrieNode('\0');
    }

    public void Add(string key, TItem item)
    {
        var invalidIndex = key.IndexOfAny(TrieHelpers.InvalidChars);
        if (invalidIndex != -1)
            throw new InvalidCharsInKeyException(key[invalidIndex]);

        var keys = key.AsSpan();
        _root.Add(keys, item);
    }

    public IEnumerable<TItem> Get(string key)
    {
        var keys = key.AsSpan();
        var node = _root;
        foreach (var c in keys)
        {
            node = node.GetChild(c);
            if (node == null)
                return Enumerable.Empty<TItem>();
        }

        return node.Items;
    }

    public IEnumerable<TItem> GetPrefix(string prefix)
    {
        var keys = prefix.AsSpan();
        var node = _root;
        foreach (var c in keys)
        {
            node = node.GetChild(c);
            if (node == null)
                return Enumerable.Empty<TItem>();
        }

        var list = new List<TItem>();
        GetPrefixRecursive(node, list);
        return list;
    }

    private void GetPrefixRecursive(ArrayTrieNode node, List<TItem> items)
    {
        items.AddRange(node.Items);

        foreach (var child in node.Children)
        {
            GetPrefixRecursive(child, items);
        }
    }

    public bool Contains(string key)
    {
        var keys = key.AsSpan();
        var node = _root;
        foreach (var c in keys)
        {
            node = node.GetChild(c);
            if (node == null)
                return false;
        }

        return node.Items.Count > 0;
    }

    public bool ContainsPrefix(string prefix)
    {
        var keys = prefix.AsSpan();
        var node = _root;
        foreach (var c in keys)
        {
            node = node.GetChild(c);
            if (node == null)
                return false;
        }

        return true;
    }

    public bool Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        return Remove(_root, key.AsSpan());
    }

    /// <summary>
    /// Method to remove an item from the trie. This method will remove all instances of the item from the trie.
    /// It is much more efficient to use the Remove(string key) method if you know the key of the item you want to remove.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(TItem item)
    {
        return Remove(_root, item);
    }

    private bool Remove(ArrayTrieNode node, TItem item)
    {
        var removed = false;
        for (var i = node.Items.Count - 1; i >= 0; i--)
        {
            if (!node.Items[i].Equals(item)) continue;
            node.Items.RemoveAt(i);
            removed = true;
        }

        for (int i = node.Children.Length - 1; i >= 0; i--)
        {
            if (!Remove(node.Children[i], item)) continue;
            removed = true;

            if (node.Children[i].Items.Count == 0 && node.Children[i].Children.Length == 0)
            {
                RemoveChildNode(node, node.Children[i].Char);
            }
        }

        return removed;
    }


    private bool Remove(ArrayTrieNode node, ReadOnlySpan<char> keySpan)
    {
        if (keySpan.Length == 0)
        {
            if (node.Items.Count <= 0) return false;
            node.Items.Clear();
            return true;
        }

        var child = node.GetChild(keySpan[0]);
        if (child == null)
            return false;

        bool isRemoved = Remove(child, keySpan[1..]);

        if (isRemoved && child.Items.Count == 0 && child.Children.Length == 0)
        {
            RemoveChildNode(node, child.Char);
        }

        return isRemoved;
    }

    private void RemoveChildNode(ArrayTrieNode parentNode, char childChar)
    {
        var index = Array.BinarySearch(parentNode.Children, new ArrayTrieNode(childChar), Comparer<ArrayTrieNode>.Create((x, y) => x.Char.CompareTo(y.Char)));

        if (index < 0) return;
        Array.Copy(parentNode.Children, index + 1, parentNode.Children, index, parentNode.Children.Length - index - 1);
        Array.Resize(ref parentNode.Children, parentNode.Children.Length - 1);
    }


    public void Clear()
    {
        _root = new ArrayTrieNode('\0');
    }

    private class ArrayTrieNode : TrieNode<ArrayTrieNode, TItem>
    {
        internal ArrayTrieNode[] Children;
        internal readonly List<TItem> Items;

        public ArrayTrieNode(char c) : base(c)
        {
            Children = Array.Empty<ArrayTrieNode>();
            Items = new List<TItem>();
        }

        public override ArrayTrieNode AddChild(char c)
        {
            var child = GetChild(c);
            if (child != null) return child;

            Array.Resize(ref Children, Children.Length + 1);

            var index = Array.BinarySearch(Children, 0, Children.Length - 1, new ArrayTrieNode(c), Comparer<ArrayTrieNode>.Create((x, y) => x.Char.CompareTo(y.Char)));
            var insertionIndex = ~index;

            Array.Copy(Children, insertionIndex, Children, insertionIndex + 1, Children.Length - insertionIndex - 1);
            Children[insertionIndex] = new ArrayTrieNode(c);
            return Children[insertionIndex];
        }


        public override ArrayTrieNode? GetChild(char c)
        {
            var index = Array.BinarySearch(Children, new ArrayTrieNode(c), Comparer<ArrayTrieNode>.Create((x, y) => x.Char.CompareTo(y.Char)));

            return index >= 0 ? Children[index] : null;
        }

        public override void Add(ReadOnlySpan<char> keys, TItem item)
        {
            if (keys.Length == 1)
            {
                var child = AddChild(keys[0]);
                child.AddItem(item);
                return;
            }

            var nextChild = AddChild(keys[0]);
            nextChild.Add(keys[1..], item);
        }

        private void AddItem(TItem item)
        {
            Items.Add(item);
        }
    }
}