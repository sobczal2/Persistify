using System;

namespace Persistify.DataStructures.Tries.Abstractions;

public abstract class TrieNode<TSelf, TItem>
    where TSelf : TrieNode<TSelf, TItem>
{
    protected TrieNode(char c)
    {
        Char = c;
    }

    public char Char { get; }
    public abstract TSelf AddChild(char c);
    public abstract TSelf? GetChild(char c);
    public abstract void Add(ReadOnlySpan<char> keys, TItem item);
}