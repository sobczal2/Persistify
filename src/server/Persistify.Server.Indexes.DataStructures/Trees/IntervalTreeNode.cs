namespace Persistify.Server.Indexes.DataStructures.Trees;

public class IntervalTreeNode<TItem>
{
    public IntervalTreeNode(TItem item, int height)
    {
        Item = item;
        Height = height;
    }

    public TItem Item { get; }
    public IntervalTreeNode<TItem>? Left { get; set; }
    public IntervalTreeNode<TItem>? Right { get; set; }
    public int Height { get; set; }
}
