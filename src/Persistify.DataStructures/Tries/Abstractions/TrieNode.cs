using System;

namespace Persistify.DataStructures.Tries.Abstractions;

public abstract class TrieNode<TSelf, TItem>
    where TSelf : TrieNode<TSelf, TItem>
{
    public char Char { get; }
    public abstract TSelf AddChild(char c);
    public abstract TSelf AddChild(char c, TItem item);
    public abstract TSelf? GetChild(char c);
    public abstract TSelf[] Children { get; }
    public TItem[] Items;
    public bool IsLeaf => Items.Length > 0;

    protected TrieNode(char c)
    {
        Char = c;
        Items = Array.Empty<TItem>();
    }
    
    protected TrieNode(char c, TItem item)
    {
        Char = c;
        Items = new[] { item };
    }

    public abstract void Add(ReadOnlySpan<char> keys, TItem item);
}