using System;
using System.Collections.Generic;
using System.Threading;

namespace Persistify.DataStructures.IntervalTree;

// switch to AVL tree
public class IntervalTree<TValue> : IIntervalTree<TValue>
    where TValue : IComparable<TValue>, IComparable<double>
{
    private readonly ReaderWriterLockSlim _lock;
    private IntervalTreeNode<TValue>? _root;

    public IntervalTree()
    {
        _lock = new ReaderWriterLockSlim();
    }

    public void Add(TValue item)
    {
        _lock.EnterWriteLock();
        try
        {
            _root = Add(_root, item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }


    public ICollection<TValue> Search(double min, double max)
    {
        ICollection<TValue> result;
        _lock.EnterReadLock();
        try
        {
            result = Search(_root, min, max);
        }
        finally
        {
            _lock.ExitReadLock();
        }

        return result;
    }

    public void Remove(Predicate<TValue> predicate)
    {
        _lock.EnterWriteLock();
        try
        {
            _root = Remove(_root, predicate);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }


    private static IntervalTreeNode<TValue>? Add(IntervalTreeNode<TValue>? node, TValue value)
    {
        if (node == null)
        {
            return new IntervalTreeNode<TValue>(value);
        }

        var compare = value.CompareTo(node.Value);
        switch (compare)
        {
            case < 0:
                node.Left = Add(node.Left, value);
                break;
            case > 0:
                node.Right = Add(node.Right, value);
                break;
        }

        return node;
    }

    private static ICollection<TValue> Search(IntervalTreeNode<TValue>? node, double min, double max)
    {
        if (node == null)
        {
            return Array.Empty<TValue>();
        }

        var values = new List<TValue>();
        if (node.Value.CompareTo(min) >= 0 && node.Value.CompareTo(max) <= 0)
        {
            values.Add(node.Value);
        }

        if (node.Left != null && node.Left.Value.CompareTo(min) >= 0)
        {
            values.AddRange(Search(node.Left, min, max));
        }

        if (node.Right != null && node.Right.Value.CompareTo(max) <= 0)
        {
            values.AddRange(Search(node.Right, min, max));
        }

        return values;
    }

    private static IntervalTreeNode<TValue>? Remove(IntervalTreeNode<TValue>? node, Predicate<TValue> predicate)
    {
        while (true)
        {
            if (node == null)
            {
                return null;
            }

            if (predicate(node.Value))
            {
                switch ((node.Left, node.Right))
                {
                    case (null, null):
                        return null;
                    case (null, _):
                        node = node.Right;
                        continue;
                    case (_, null):
                        node = node.Left;
                        continue;
                }
            }
            else
            {
                node.Left = Remove(node.Left, predicate);
                node.Right = Remove(node.Right, predicate);
            }

            return node;
        }
    }
}
