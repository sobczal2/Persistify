using Persistify.DataStructures.Trees;

namespace Persistify.DataStructures.Test.Trees;

public static class TreeTestExtensions
{
    public static TreeNode<T>? GetRoot<T>(this ConcurrentIntervalTree<T> concurrentIntervalTree)
    {
        return (TreeNode<T>?)concurrentIntervalTree.GetType().GetField("_root", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(concurrentIntervalTree);
    }
}