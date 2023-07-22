using System;
using System.Collections.Generic;
using System.Reflection;
using Persistify.DataStructures.PrefixTree;

namespace Persistify.DataStructures.Test;

public static class PrefixTreeTestExtensions
{
    public static TrieNode<TValue> GetRoot<TValue>(this Trie<TValue> trie)
    {
        return typeof(Trie<TValue>)
            .GetField("_root", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(trie) as TrieNode<TValue> ?? throw new InvalidOperationException();
    }

    public static ICollection<TValue> GetAllValues<TValue>(this Trie<TValue> trie)
    {
        var root = trie.GetRoot();
        var values = new List<TValue>();
        typeof(Trie<TValue>)
            .GetMethod("CollectAllValues", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, new object[] { root, values });

        return values;
    }
}
