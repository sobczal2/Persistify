using System.Collections.Generic;

namespace Persistify.DataStructures.Tries.ByteTranslationFixedSizeTrie;

public struct ByteTranslationFixedSizeTrieNode<TItem>
{
    private readonly ByteTranslationFixedSizeTrieNode<TItem>?[] _children;
    private readonly List<TItem> _items;
    public IReadOnlyList<TItem> Items => _items;

    public ByteTranslationFixedSizeTrieNode(byte size)
    {
        _children = new ByteTranslationFixedSizeTrieNode<TItem>?[size];
        _items = new List<TItem>();
    }

    public ByteTranslationFixedSizeTrieNode<TItem> GetOrAddChild(byte b)
    {
        _children[b] ??= new ByteTranslationFixedSizeTrieNode<TItem>((byte)_children.Length);

        return _children[b]!.Value;
    }

    public ByteTranslationFixedSizeTrieNode<TItem>? GetChild(byte b)
    {
        return _children[b];
    }

    public IEnumerable<ByteTranslationFixedSizeTrieNode<TItem>> GetActiveChildren()
    {
        for (var i = 0; i < _children.Length; i++)
            if (_children[i] != null)
                yield return _children[i]!.Value;
    }

    public void AddItem(TItem item)
    {
        _items.Add(item);
    }
}