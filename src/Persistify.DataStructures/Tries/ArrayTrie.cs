using System;
using System.Collections.Generic;
using System.Linq;
using Persistify.DataStructures.Tries.Abstractions;
using Persistify.DataStructures.Tries.Exceptions;

namespace Persistify.DataStructures.Tries;

public class ArrayTrie<TItem> : ITrie<TItem>
{
    public class ArrayTrieNode : TrieNode<ArrayTrieNode, TItem>
    {
        private ArrayTrieNode[] _children;

        public ArrayTrieNode(char c) : base(c)
        {
            _children = Array.Empty<ArrayTrieNode>();
        }
        
        public ArrayTrieNode(char c, TItem item) : base(c, item)
        {
            _children = Array.Empty<ArrayTrieNode>();
        }

        public override ArrayTrieNode[] Children => _children;

        public override ArrayTrieNode AddChild(char c)
        {
            var child = GetChild(c);
            if (child != null)
            {
                return child;
            }

            Array.Resize(ref _children, _children.Length + 1);
            _children[^1] = new ArrayTrieNode(c);
            return _children[^1];
        }
        
        public override ArrayTrieNode AddChild(char c, TItem item)
        {
            var child = GetChild(c);
            if (child != null)
            {
                child.AddItem(item);
                return child;
            }

            Array.Resize(ref _children, _children.Length + 1);
            _children[^1] = new ArrayTrieNode(c, item);
            return _children[^1];
        }

        public override ArrayTrieNode? GetChild(char c)
        {
            ArrayTrieNode? child = null;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _children.Length; i++)
            {
                if (_children[i].Char != c) continue;
                child = _children[i];
                break;
            }

            return child;
        }

        private void AddItem(TItem item)
        {
            Array.Resize(ref Items, Items.Length + 1);
            Items[^1] = item;
        }

        public override void Add(ReadOnlySpan<char> keys, TItem item)
        {
            if (keys.Length == 1)
            {
                AddChild(keys[0], item);
                return;
            }

            var child = AddChild(keys[0]);
            child.Add(keys[1..], item);
        }
    }

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
        var list = new List<TItem>();
        var keys = prefix.AsSpan();
        var node = _root;
        for(var i = 0; i < keys.Length - 1; i++)
        {
            node = node.GetChild(keys[i]);
            if (node == null)
                return Enumerable.Empty<TItem>();
        }

        list.AddRange(node.Items);
        for (var i = 0; i < node.Children.Length; i++)
        {
            list.AddRange(GetPrefix(prefix + node.Children[i].Char));
        }

        return list;
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

        return node.IsLeaf;
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
        throw new NotImplementedException();
    }

    public void Clear()
    {
        _root = new ArrayTrieNode('\0');
    }
}