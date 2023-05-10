using System.Reflection;
using Persistify.DataStructures.Trees;

namespace Persistify.DataStructures.Test.Trees;

public static class TreeTestExtensions
{
    public static TreeNode<T>? GetRoot<T>(this ConcurrentIntervalTree<T> concurrentIntervalTree)
    {
        return (TreeNode<T>?)concurrentIntervalTree.GetType()
            .GetField("_root", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(concurrentIntervalTree);
    }
}