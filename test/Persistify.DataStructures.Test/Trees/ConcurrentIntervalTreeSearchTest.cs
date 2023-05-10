using System.Linq;
using Persistify.Extensions;
using Xunit;

namespace Persistify.DataStructures.Test.Trees;

public class ConcurrentIntervalTreeSearchTest : TreeTestBase
{
    [Fact]
    public void Search_WhenTreeIsEmpty_ShouldReturnEmptyList()
    {
        var result = Tree.Search(0, 1);

        Assert.Empty(result);
    }

    [Fact]
    public void Search_WhenTreeHasOneNode_ShouldReturnEmptyList()
    {
        Tree.Insert("A", 1);

        var result = Tree.Search(0, 0.5);

        Assert.Empty(result);
    }

    [Fact]
    public void Search_WhenTreeHasOneNode_ShouldReturnListWithOneItem()
    {
        Tree.Insert("A", 1);

        var result = Tree.Search(0.5, 1.5).ToListOptimized();

        Assert.Single(result);
        Assert.Equal("A", result.First());
    }

    [Fact]
    public void Search_WhenTreeHasTwoNodes_ShouldReturnListWithTwoItems()
    {
        Tree.Insert("A", 1);
        Tree.Insert("B", 2);

        var result = Tree.Search(0.5, 2.5).ToListOptimized();

        Assert.Equal(2, result.Count);
        Assert.Equal("A", result.First());
        Assert.Equal("B", result.Last());
    }

    [Fact]
    public void Search_WhenTreeHasTwoNodes_ShouldReturnListWithOneItem()
    {
        Tree.Insert("A", 1);
        Tree.Insert("B", 2);

        var result = Tree.Search(1.5, 2.5).ToListOptimized();

        Assert.Single(result);
        Assert.Equal("B", result.First());
    }
}