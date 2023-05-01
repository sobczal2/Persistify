using Persistify.DataStructures.Trees;

namespace Persistify.DataStructures.Test.Trees;

public class TreeTestBase
{
    protected ConcurrentIntervalTree<string> Tree;

    public TreeTestBase()
    {
        Tree = new ConcurrentIntervalTree<string>();
    }
}