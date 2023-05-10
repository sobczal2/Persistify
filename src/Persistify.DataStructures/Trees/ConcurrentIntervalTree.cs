using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Persistify.Helpers;

namespace Persistify.DataStructures.Trees;

public class ConcurrentIntervalTree<TItem> : ITree<TItem>
{
    private readonly ReaderWriterLockSlim _lock;
    [JsonProperty] private TreeNode<TItem>? _root;

    public ConcurrentIntervalTree()
    {
        _lock = new ReaderWriterLockSlim();
    }

    public void Insert(TItem item, double value)
    {
        _lock.EnterWriteLock();
        try
        {
            _root = Insert(_root, item, value);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public IEnumerable<TItem> Search(double min, double max)
    {
        _lock.EnterReadLock();
        try
        {
            var result = new List<TItem>();
            Search(_root, min, max, result);
            return result;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public int Remove(Predicate<TItem> predicate)
    {
        _lock.EnterWriteLock();
        try
        {
            var removedCount = 0;
            _root = Remove(_root, predicate, ref removedCount);
            return removedCount;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private static TreeNode<TItem> Insert(TreeNode<TItem>? node, TItem item, double value)
    {
        if (node == null)
            return new TreeNode<TItem>(value, item, 1);

        if (value < node.Value)
            node.Left = Insert(node.Left, item, value);
        else
            node.Right = Insert(node.Right, item, value);

        UpdateNodeHeight(node);

        return BalanceNode(node);
    }

    private static void Search(TreeNode<TItem>? node, double min, double max, ICollection<TItem> result)
    {
        if (node == null)
            return;

        if (min <= node.Value)
            Search(node.Left, min, max, result);

        if (min <= node.Value && max >= node.Value)
            result.Add(node.Item);

        if (max >= node.Value)
            Search(node.Right, min, max, result);
    }

    private TreeNode<TItem>? Remove(TreeNode<TItem>? node, Predicate<TItem> predicate, ref int removedCount)
    {
        if (node == null)
            return null;

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

    private TreeNode<TItem>? RemoveNode(TreeNode<TItem> node)
    {
        if (node.Left == null)
            return node.Right;

        if (node.Right == null)
            return node.Left;

        var minNode = FindMinNode(node.Right);
        minNode.Right = RemoveMinNode(node.Right);
        minNode.Left = node.Left;
        UpdateNodeHeight(minNode);

        return BalanceNode(minNode);
    }

    private TreeNode<TItem>? RemoveMinNode(TreeNode<TItem> node)
    {
        if (node.Left == null)
            return node.Right;

        node.Left = RemoveMinNode(node.Left);
        UpdateNodeHeight(node);

        return BalanceNode(node);
    }

    private static TreeNode<TItem> FindMinNode(TreeNode<TItem> node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }


    private static void UpdateNodeHeight(TreeNode<TItem> node)
    {
        node.Height = 1 + MathI.Max(GetHeight(node.Left), GetHeight(node.Right));
    }

    private static TreeNode<TItem> BalanceNode(TreeNode<TItem> node)
    {
        var balance = GetBalance(node);

        switch (balance)
        {
            case > 1:
            {
                if (GetBalance(node.Left) < 0) node.Left = LeftRotate(node.Left!);

                node = RightRotate(node);
                break;
            }
            case < -1:
            {
                if (GetBalance(node.Right) > 0) node.Right = RightRotate(node.Right!);

                node = LeftRotate(node);
                break;
            }
        }

        return node;
    }

    private static int GetBalance(TreeNode<TItem>? node)
    {
        if (node == null) return 0;

        return GetHeight(node.Left) - GetHeight(node.Right);
    }

    private static int GetHeight(TreeNode<TItem>? node)
    {
        return node?.Height ?? 0;
    }

    private static TreeNode<TItem> RightRotate(TreeNode<TItem> node)
    {
        var leftChild = node.Left ?? throw new InvalidOperationException("Left child is null");
        node.Left = leftChild.Right;
        leftChild.Right = node;
        UpdateNodeHeight(node);
        UpdateNodeHeight(leftChild);

        return leftChild;
    }

    private static TreeNode<TItem> LeftRotate(TreeNode<TItem> node)
    {
        var rightChild = node.Right ?? throw new InvalidOperationException("Right child is null");
        node.Right = rightChild.Left;
        rightChild.Left = node;

        UpdateNodeHeight(node);
        UpdateNodeHeight(rightChild);

        return rightChild;
    }
}