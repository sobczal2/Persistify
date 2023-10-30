using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Trees;

public class IntervalTree<TItem> : ITree<TItem>
    where TItem : IComparable<TItem>
{
    private IntervalTreeNode<TItem>? _root;

    public void Insert(TItem item)
    {
        _root = Insert(_root, item);
    }

    public List<TItem> Search<TValue>(TValue min, TValue max, Func<TItem, TValue, int> comparer)
    {
        var result = new List<TItem>();
        Search(_root, min, max, comparer, result);
        return result;
    }

    public int Remove(Predicate<TItem> predicate)
    {
        var removedCount = 0;
        _root = Remove(_root, predicate, ref removedCount);
        return removedCount;
    }

    private static IntervalTreeNode<TItem> Insert(IntervalTreeNode<TItem>? node, TItem item)
    {
        if (node == null)
        {
            return new IntervalTreeNode<TItem>(item, 1);
        }

        if (item.CompareTo(node.Item) < 0)
        {
            node.Left = Insert(node.Left, item);
        }
        else
        {
            node.Right = Insert(node.Right, item);
        }

        UpdateNodeHeight(node);

        return BalanceNode(node);
    }

    private static void Search<TValue>(
        IntervalTreeNode<TItem>? node,
        TValue min,
        TValue max,
        Func<TItem, TValue, int> comparer,
        ICollection<TItem> result
    )
    {
        if (node is null)
        {
            return;
        }

        if (comparer(node.Item, min) < 0)
        {
            Search(node.Left, min, max, comparer, result);
        }

        if (comparer(node.Item, max) > 0)
        {
            Search(node.Right, min, max, comparer, result);
        }

        if (comparer(node.Item, min) >= 0 && comparer(node.Item, max) <= 0)
        {
            result.Add(node.Item);
        }
    }

    private IntervalTreeNode<TItem>? Remove(
        IntervalTreeNode<TItem>? node,
        Predicate<TItem> predicate,
        ref int removedCount
    )
    {
        if (node == null)
        {
            return null;
        }

        node.Left = Remove(node.Left, predicate, ref removedCount);
        node.Right = Remove(node.Right, predicate, ref removedCount);

        if (predicate(node.Item))
        {
            removedCount++;
            return RemoveNode(node);
        }

        UpdateNodeHeight(node);

        return BalanceNode(node);
    }

    private IntervalTreeNode<TItem>? RemoveNode(IntervalTreeNode<TItem> node)
    {
        if (node.Left == null)
        {
            return node.Right;
        }

        if (node.Right == null)
        {
            return node.Left;
        }

        var minNode = FindMinNode(node.Right);
        minNode.Right = RemoveMinNode(node.Right);
        minNode.Left = node.Left;
        UpdateNodeHeight(minNode);

        return BalanceNode(minNode);
    }

    private IntervalTreeNode<TItem>? RemoveMinNode(IntervalTreeNode<TItem> node)
    {
        if (node.Left == null)
        {
            return node.Right;
        }

        node.Left = RemoveMinNode(node.Left);
        UpdateNodeHeight(node);

        return BalanceNode(node);
    }

    private static IntervalTreeNode<TItem> FindMinNode(IntervalTreeNode<TItem> node)
    {
        while (node.Left != null)
        {
            node = node.Left;
        }

        return node;
    }

    private static void UpdateNodeHeight(IntervalTreeNode<TItem> node)
    {
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
    }

    private static IntervalTreeNode<TItem> BalanceNode(IntervalTreeNode<TItem> node)
    {
        var balance = GetBalance(node);

        switch (balance)
        {
            case > 1:
            {
                if (GetBalance(node.Left) < 0)
                {
                    node.Left = LeftRotate(node.Left!);
                }

                node = RightRotate(node);
                break;
            }
            case < -1:
            {
                if (GetBalance(node.Right) > 0)
                {
                    node.Right = RightRotate(node.Right!);
                }

                node = LeftRotate(node);
                break;
            }
        }

        return node;
    }

    private static int GetBalance(IntervalTreeNode<TItem>? node)
    {
        if (node == null)
        {
            return 0;
        }

        return GetHeight(node.Left) - GetHeight(node.Right);
    }

    private static int GetHeight(IntervalTreeNode<TItem>? node)
    {
        return node?.Height ?? 0;
    }

    private static IntervalTreeNode<TItem> RightRotate(IntervalTreeNode<TItem> node)
    {
        var leftChild = node.Left ?? throw new InvalidOperationException("Left child is null");
        node.Left = leftChild.Right;
        leftChild.Right = node;
        UpdateNodeHeight(node);
        UpdateNodeHeight(leftChild);

        return leftChild;
    }

    private static IntervalTreeNode<TItem> LeftRotate(IntervalTreeNode<TItem> node)
    {
        var rightChild = node.Right ?? throw new InvalidOperationException("Right child is null");
        node.Right = rightChild.Left;
        rightChild.Left = node;

        UpdateNodeHeight(node);
        UpdateNodeHeight(rightChild);

        return rightChild;
    }
}
