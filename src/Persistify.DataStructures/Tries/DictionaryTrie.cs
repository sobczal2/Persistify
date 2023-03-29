using System;
using System.Collections.Generic;
using System.Linq;
using Persistify.DataStructures.Tries.Abstractions;
using Persistify.DataStructures.Tries.Exceptions;

namespace Persistify.DataStructures.Tries;

public class DictionaryTrie<TItem> : ITrie<TItem> where TItem : struct, IComparable<TItem>, IConvertible
{
    private readonly DictionaryTrieNode _root;

    public DictionaryTrie()
    {
        _root = new DictionaryTrieNode('\0');
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
        return GetNode(key)?.Items ?? Enumerable.Empty<TItem>();
    }

    public IEnumerable<TItem> GetPrefix(string prefix)
    {
        var items = new List<TItem>();
        if (GetNode(prefix) is { } node)
        {
            GetAllItems(node, items);
        }
        return items;
    }

    private DictionaryTrieNode? GetNode(string key)
    {
        var keys = key.AsSpan();
        var node = _root;

        foreach (var c in keys)
        {
            node = node.GetChild(c);
            if (node == null) return null;
        }

        return node;
    }

    private static void GetAllItems(DictionaryTrieNode node, List<TItem> items)
    {
        var stack = new Stack<DictionaryTrieNode>();
        stack.Push(node);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            items.AddRange(currentNode.Items);

            foreach (var child in currentNode.Children.Values)
                stack.Push(child);
        }
    }


    public bool Contains(string key)
    {
        var node = GetNode(key);
        return node is { Items.Count: > 0 };
    }

    public bool ContainsPrefix(string prefix)
    {
        return GetNode(prefix) != null;
    }

    public bool Remove(string key)
    {
        throw new NotImplementedException();
    }

    public bool Remove(TItem item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        _root.Clear();
    }

    public class DictionaryTrieNode : TrieNode<DictionaryTrieNode, TItem>
    {
        internal readonly Dictionary<char, DictionaryTrieNode> Children;
        internal readonly List<TItem> Items;

        public DictionaryTrieNode(char c) : base(c)
        {
            Children = new Dictionary<char, DictionaryTrieNode>();
            Items = new List<TItem>();
        }

        public void Clear()
        {
            Children.Clear();
            Items.Clear();
        }

        public override DictionaryTrieNode AddChild(char c)
        {
            return Children.TryGetValue(c, out var child) ? child : Children[c] = new DictionaryTrieNode(c);
        }

        public override DictionaryTrieNode? GetChild(char c)
        {
            Children.TryGetValue(c, out var child);
            return child;
        }

        public override void Add(ReadOnlySpan<char> keys, TItem item)
        {
            if (keys.IsEmpty) return;

            var child = AddChild(keys[0]);
            if (keys.Length == 1)
                child.Items.Add(item);
            else
                child.Add(keys[1..], item);
        }
    }
}
