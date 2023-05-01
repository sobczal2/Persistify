using System.Collections.Generic;
using Persistify.DataStructures.Tries;

namespace Persistify.DataStructures.Test.Tries;

public static class TrieTestExtensions
{
    public static TrieNode<T>? GetRoot<T>(this ConcurrentByteMapTrie<T> concurrentIntervalTree)
    {
        return (TrieNode<T>?)concurrentIntervalTree.GetType().GetField("_root", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(concurrentIntervalTree);
    }
    
    public static List<TItem>? GetItems<TItem>(this TrieNode<TItem> node)
    {
        return (List<TItem>?)node.GetType().GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(node);
    }
    
    public static TrieNode<TItem>?[]? GetChildren<TItem>(this TrieNode<TItem> node)
    {
        return (TrieNode<TItem>?[]?)node.GetType().GetField("_children", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(node);
    }
}