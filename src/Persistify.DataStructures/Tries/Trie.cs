using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Persistify.DataStructures.Tries;

public class Trie<TItem> : ITrie<TItem>
{
    [JsonProperty]
    private readonly TrieNode<TItem> _root;

    public Trie()
    {
        _root = new TrieNode<TItem>();
    }
    public void Add(string key, TItem item)
    {
        _root.Add(key.AsSpan(), item);
    }

    public IEnumerable<TItem> Search(string key)
    {
        return _root.Search(key.AsSpan());
    }

    public int Remove(Predicate<TItem> predicate)
    {
        return _root.Remove(predicate);
    }
}