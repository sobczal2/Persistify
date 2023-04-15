using System;
using System.Collections.Generic;
using System.Linq;

namespace Persistify.DataStructures.Tries.ByteTranslationFixedSizeTrie;

public delegate byte CharByteTranslation(char c);

public delegate Span<byte> StringByteTranslation(string s);

public class ByteTranslationFixedSizeTrie<TItem> : ITrie<TItem>
{
    private readonly ByteTranslationFixedSizeTrieNode<TItem> _root;
    private readonly StringByteTranslation _stringByteTranslation;

    public ByteTranslationFixedSizeTrie(StringByteTranslation stringByteTranslation, byte size)
    {
        _stringByteTranslation = stringByteTranslation;
        _root = new ByteTranslationFixedSizeTrieNode<TItem>(size);
        UniqueItemsCount = 0;
    }

    public ByteTranslationFixedSizeTrie(CharByteTranslation charByteTranslation, byte size)
    {
        _stringByteTranslation = s => s.ToCharArray().Select(x => charByteTranslation(x)).ToArray();
        _root = new ByteTranslationFixedSizeTrieNode<TItem>(size);
        UniqueItemsCount = 0;
    }

    public long UniqueItemsCount { get; private set; }

    public void Add(string key, TItem item)
    {
        var bytes = _stringByteTranslation(key);
        var node = _root;
        for (var i = 0; i < bytes.Length - 1; i++)
            node = node.GetOrAddChild(bytes[i]);

        node.AddItem(item);
        UniqueItemsCount++;
    }

    public void AddRange(IEnumerable<KeyValuePair<string, TItem>> items)
    {
        // TODO: Aggregate same keys
        foreach (var (key, item) in items)
            Add(key, item);
    }

    public IEnumerable<TItem> Get(string key)
    {
        var bytes = _stringByteTranslation(key);
        ByteTranslationFixedSizeTrieNode<TItem>? node = _root;
        for (var i = 0; i < bytes.Length - 1; i++)
        {
            node = node.Value.GetChild(bytes[i]);
            if (node == null)
                yield break;
        }

        foreach (var item in node.Value.Items)
            yield return item;
    }

    public IEnumerable<TItem> Search(string query)
    {
        var bytes = _stringByteTranslation(query);
        ByteTranslationFixedSizeTrieNode<TItem>? node = _root;
        for (var i = 0; i < bytes.Length - 1; i++)
        {
            node = node.Value.GetChild(bytes[i]);
            if (node == null)
                yield break;
        }

        foreach (var item in node.Value.Items)
            yield return item;

        foreach (var child in node.Value.GetActiveChildren())
        foreach (var item in child.Items)
            yield return item;
    }

    public bool Contains(string key)
    {
        var bytes = _stringByteTranslation(key);
        ByteTranslationFixedSizeTrieNode<TItem>? node = _root;
        for (var i = 0; i < bytes.Length - 1; i++)
        {
            node = node.Value.GetChild(bytes[i]);
            if (node == null)
                return false;
        }

        return node.Value.Items.Count > 0;
    }

    public bool Remove(TItem item)
    {
        throw new NotImplementedException();
    }

    public TrieStats GetStats()
    {
        throw new NotImplementedException();
    }
}