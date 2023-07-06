using System;
using System.Collections.Generic;
using System.Reflection;
using Persistify.DataStructures.IntervalTree;

namespace Persistify.DataStructures.Test;

public static class IntervalTreeTestExtensions
{
    public static IntervalTreeNode<TValue> GetRoot<TValue>(this IntervalTree<TValue> intervalTree)
        where TValue : IComparable<TValue>, IComparable<double>
    {
        return typeof(IntervalTree<TValue>)
            .GetField("_root", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(intervalTree) as IntervalTreeNode<TValue> ?? throw new InvalidOperationException();
    }

    public static ICollection<TValue> GetAllValues<TValue>(this IntervalTree<TValue> prefixTree)
        where TValue : IComparable<TValue>, IComparable<double>
    {
        var root = prefixTree.GetRoot();
        var values = new List<TValue>();

        CollectAllValues(root, values);

        return values;
    }

    private static void CollectAllValues<TValue>(IntervalTreeNode<TValue> node, ICollection<TValue> values)
        where TValue : IComparable<TValue>, IComparable<double>
    {
        if (node.Left != null)
        {
            CollectAllValues(node.Left, values);
        }

        if (node.Right != null)
        {
            CollectAllValues(node.Right, values);
        }

        values.Add(node.Value);
    }
}
