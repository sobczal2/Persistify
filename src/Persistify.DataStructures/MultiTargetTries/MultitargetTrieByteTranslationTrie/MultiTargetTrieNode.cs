using System.Collections.Generic;

namespace Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie;

public class MultiTargetTrieNode<TItem>
    where TItem : notnull
{
    private readonly byte _alphabetSize;
    private readonly MultiTargetTrieNode<TItem>?[] _children;
    private readonly List<TItem> _items;
    public IReadOnlyList<TItem> Items => GetAllItems();

    public MultiTargetTrieNode(byte alphabetSize)
    {
        _alphabetSize = alphabetSize;
        _children = new MultiTargetTrieNode<TItem>[alphabetSize];
        _items = new List<TItem>();
    }

    public void AddItem(TItem item)
    {
        _items.Add(item);
    }
    
    public MultiTargetTrieNode<TItem> GetOrAddChild(byte index)
    {
        _children[index] ??= new MultiTargetTrieNode<TItem>(_alphabetSize);
        return _children[index]!;
    }
    
    public MultiTargetTrieNode<TItem>? GetChild(byte index)
    {
        return _children[index];
    }
    
    private List<TItem> GetAllItems()
    {
        var result = new List<TItem>();
        result.AddRange(_items);
        foreach (var child in _children)
        {
            if (child != null)
                result.AddRange(child.GetAllItems());
        }
        return result;
    }
}