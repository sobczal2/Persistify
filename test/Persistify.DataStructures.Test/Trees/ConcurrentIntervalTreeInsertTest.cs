using Persistify.DataStructures.Trees;
using Xunit;

namespace Persistify.DataStructures.Test.Trees;

public class ConcurrentIntervalTreeInsertTest : TreeTestBase
{


    [Fact]
    public void Insert_WhenTreeIsEmpty_ShouldInsertRootNode()
    {
        Tree.Insert("A", 1);
        
        var root = Tree.GetRoot();
        
        Assert.NotNull(root);
        Assert.Equal("A", root.Item);
        Assert.Equal(1, root.Value);
        Assert.Equal(1, root.Height);
        Assert.Null(root.Left);
        Assert.Null(root.Right);
    }
    
    [Fact]
    public void Insert_WhenTreeHasOneNode_ShouldInsertLeftNode()
    {
        Tree.Insert("A", 1);
        Tree.Insert("B", 0);
        
        var root = Tree.GetRoot();
        
        Assert.NotNull(root);
        Assert.Equal("A", root.Item);
        Assert.Equal(1, root.Value);
        Assert.Equal(2, root.Height);
        Assert.NotNull(root.Left);
        Assert.Null(root.Right);
        
        Assert.Equal("B", root.Left.Item);
        Assert.Equal(0, root.Left.Value);
        Assert.Equal(1, root.Left.Height);
        Assert.Null(root.Left.Left);
        Assert.Null(root.Left.Right);
    }
    
    [Fact]
    public void Insert_WhenTreeHasOneNode_ShouldInsertRightNode()
    {
        Tree.Insert("A", 1);
        Tree.Insert("B", 2);
        
        var root = Tree.GetRoot();
        
        Assert.NotNull(root);
        Assert.Equal("A", root.Item);
        Assert.Equal(1, root.Value);
        Assert.Equal(2, root.Height);
        Assert.Null(root.Left);
        Assert.NotNull(root.Right);
        
        Assert.Equal("B", root.Right.Item);
        Assert.Equal(2, root.Right.Value);
        Assert.Equal(1, root.Right.Height);
        Assert.Null(root.Right.Left);
        Assert.Null(root.Right.Right);
    }
}