namespace Persistify.DataStructures.Trees;

public class TreeNode<TItem>
{
    public TreeNode(double value, TItem item, int height)
    {
        Value = value;
        Item = item;
        Height = height;
    }

    public double Value { get; }
    public TItem Item { get; }
    public TreeNode<TItem>? Left { get; set; }
    public TreeNode<TItem>? Right { get; set; }
    public int Height { get; set; }
}