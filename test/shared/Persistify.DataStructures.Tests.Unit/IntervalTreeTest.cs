using System;
using System.Linq;
using Persistify.DataStructures.IntervalTree;
using Xunit;

namespace Persistify.DataStructures.Test;

public class IntervalTreeTest
{
    private readonly IIntervalTree<IntervalTreeValue> _tree;

    public IntervalTreeTest()
    {
        _tree = new IntervalTree<IntervalTreeValue>();
    }

    [Fact]
    public void Search_SingleItem_Success()
    {
        // Arrange
        var item = new IntervalTreeValue(1.0);
        _tree.Add(item);

        // Act
        var result = _tree.Search(0.0, 2.0);

        // Assert
        Assert.Single(result);
        Assert.Equal(item, result.First());
    }

    [Fact]
    public void Search_MultipleItems_Success()
    {
        // Arrange
        var item1 = new IntervalTreeValue(1.0);
        var item2 = new IntervalTreeValue(2.0);
        var item3 = new IntervalTreeValue(3.0);
        _tree.Add(item1);
        _tree.Add(item2);
        _tree.Add(item3);

        // Act
        var result = _tree.Search(0.0, 4.0);

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(item1, result);
        Assert.Contains(item2, result);
        Assert.Contains(item3, result);
    }

    [Fact]
    public void Search_EmptyTree_Success()
    {
        // Act
        var result = _tree.Search(0.0, 4.0);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Search_SingleItemOutOfRange_Success()
    {
        // Arrange
        var item = new IntervalTreeValue(1.0);
        _tree.Add(item);

        // Act
        var result = _tree.Search(2.0, 4.0);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Search_MultipleItemsOutOfRange_Success()
    {
        // Arrange
        var item1 = new IntervalTreeValue(1.0);
        var item2 = new IntervalTreeValue(2.0);
        var item3 = new IntervalTreeValue(3.0);
        _tree.Add(item1);
        _tree.Add(item2);
        _tree.Add(item3);

        // Act
        var result = _tree.Search(4.0, 5.0);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Search_ExactlyOnTheEdge_Success()
    {
        // Arrange
        var item1 = new IntervalTreeValue(1.0);
        var item2 = new IntervalTreeValue(2.0);
        var item3 = new IntervalTreeValue(3.0);
        _tree.Add(item1);
        _tree.Add(item2);
        _tree.Add(item3);

        // Act
        var result = _tree.Search(2.0, 3.0);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(item2, result);
        Assert.Contains(item3, result);
    }

    [Fact]
    public void Search_ExactMatch_Success()
    {
        // Arrange
        var item1 = new IntervalTreeValue(1.0);
        var item2 = new IntervalTreeValue(2.0);
        var item3 = new IntervalTreeValue(3.0);
        _tree.Add(item1);
        _tree.Add(item2);
        _tree.Add(item3);

        // Act
        var result = _tree.Search(2.0, 2.0);

        // Assert
        Assert.Single(result);
        Assert.Contains(item2, result);
    }

    [Fact]
    public void Remove_SingleItem_Success()
    {
        // Arrange
        var item = new IntervalTreeValue(1.0);
        _tree.Add(item);

        // Act
        _tree.Remove(x => x == item);

        // Assert
        Assert.Empty(_tree.Search(0.0, 2.0));
    }

    [Fact]
    public void Remove_MultipleItems_Success()
    {
        // Arrange
        var item1 = new IntervalTreeValue(1.0);
        var item2 = new IntervalTreeValue(2.0);
        var item3 = new IntervalTreeValue(3.0);
        _tree.Add(item1);
        _tree.Add(item2);
        _tree.Add(item3);

        // Act
        _tree.Remove(x => x.Value > 1.0);

        // Assert
        Assert.Single(_tree.Search(0.0, 4.0));
        Assert.Contains(item1, _tree.Search(0.0, 4.0));
    }

    private class IntervalTreeValue : IComparable<IntervalTreeValue>, IComparable<double>
    {
        public IntervalTreeValue(double value)
        {
            Value = value;
        }

        public double Value { get; }

        public int CompareTo(double other)
        {
            return Value.CompareTo(other);
        }

        public int CompareTo(IntervalTreeValue? other)
        {
            if (other is null)
            {
                return 1;
            }

            return Value.CompareTo(other.Value);
        }
    }
}
