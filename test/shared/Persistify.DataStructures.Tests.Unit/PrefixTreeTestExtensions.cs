using System;
using System.Collections.Generic;
using System.Reflection;
using Persistify.DataStructures.PrefixTree;

namespace Persistify.DataStructures.Test;

public static class PrefixTreeTestExtensions
{
    public static PrefixTreeNode<TValue> GetRoot<TValue>(this PrefixTree<TValue> prefixTree)
    {
        return typeof(PrefixTree<TValue>)
            .GetField("_root", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(prefixTree) as PrefixTreeNode<TValue> ?? throw new InvalidOperationException();
    }

    public static ICollection<TValue> GetAllValues<TValue>(this PrefixTree<TValue> prefixTree)
    {
        var root = prefixTree.GetRoot();
        var values = new List<TValue>();
        typeof(PrefixTree<TValue>)
            .GetMethod("CollectAllValues", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, new object[] { root, values });

        return values;
    }
}
