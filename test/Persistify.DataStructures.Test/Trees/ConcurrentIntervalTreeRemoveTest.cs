using Xunit;

namespace Persistify.DataStructures.Test.Trees;

public class ConcurrentIntervalTreeRemoveTest : TreeTestBase
{
    [Fact]
    public void Remove_WhenTreeIsEmpty_ShouldReturn0()
    {
        var result = Tree.Remove(x => x == "A");
        
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void Remove_WhenTreeHasOneNode_ShouldReturn1()
    {
        Tree.Insert("A", 1);
        
        var result = Tree.Remove(x => x == "A");
        
        Assert.Equal(1, result);
    }
    
    [Fact]
    public void Remove_WhenTreeHasOneNode_ShouldReturn0()
    {
        Tree.Insert("A", 1);
        
        var result = Tree.Remove(x => x == "B");
        
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void Remove_WhenTreeHasTwoNodes_ShouldReturn1()
    {
        Tree.Insert("A", 1);
        Tree.Insert("B", 2);
        
        var result = Tree.Remove(x => x == "A");
        
        Assert.Equal(1, result);
    }
    
    [Fact]
    public void Remove_WhenTreeHasTwoNodes_ShouldReturn0()
    {
        Tree.Insert("A", 1);
        Tree.Insert("B", 2);
        
        var result = Tree.Remove(x => x == "C");
        
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void Remove_WhenTreeHasTwoNodes_ShouldReturn2()
    {
        Tree.Insert("A", 1);
        Tree.Insert("B", 2);
        
        var result = Tree.Remove(x => x is "A" or "B");
        
        Assert.Equal(2, result);
    }
}