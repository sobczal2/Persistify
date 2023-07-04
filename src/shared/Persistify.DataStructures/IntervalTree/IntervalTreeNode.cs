namespace Persistify.DataStructures.IntervalTree;

public class IntervalTreeNode<TValue>
    where TValue : IComparable<TValue>, IComparable<double>
{
    public IntervalTreeNode(TValue value, IntervalTreeNode<TValue>? left = null, IntervalTreeNode<TValue>? right = null)
    {
        Value = value;
        Left = left;
        Right = right;
    }

    public TValue Value { get; set; }
    public IntervalTreeNode<TValue>? Left { get; set; }
    public IntervalTreeNode<TValue>? Right { get; set; }
}
