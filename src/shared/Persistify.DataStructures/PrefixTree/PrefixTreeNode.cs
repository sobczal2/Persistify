namespace Persistify.DataStructures.PrefixTree;

public class PrefixTreeNode<TValue>
{
    public PrefixTreeNode<TValue>?[] Children { get; set; }
    public HashSet<TValue> Values { get; set; }
    
    public PrefixTreeNode()
    {
        Children = new PrefixTreeNode<TValue>[62];
        Values = new HashSet<TValue>();
    }
}
