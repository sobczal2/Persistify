using System.Threading.Tasks;
using Persistify.Persistence.DataStructures.Abstractions;
using Xunit;

namespace Persistify.Persistence.DataStructures.Tests.Unit.Trees;

public abstract class AsyncTreeTestBase
{
    private readonly IAsyncTree<long> _tree;

    public AsyncTreeTestBase(IAsyncTree<long> tree)
    {
        _tree = tree;
    }

    [Fact]
    public async Task IndexAsync_SingleValue_ReturnsValue()
    {
        // Arrange
        const long value = 1;

        // Act
        await _tree.InsertAsync(value);
        var result = await _tree.GetAsync(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public async Task IndexAsync_MultipleValues_ReturnsValue()
    {
        // Arrange
        const long value1 = 1;
        const long value2 = 2;
        const long value3 = 3;

        // Act
        await _tree.InsertAsync(value1);
        await _tree.InsertAsync(value2);
        await _tree.InsertAsync(value3);
        var result1 = await _tree.GetAsync(value1);
        var result2 = await _tree.GetAsync(value2);
        var result3 = await _tree.GetAsync(value3);

        // Assert
        Assert.Equal(value1, result1);
        Assert.Equal(value2, result2);
        Assert.Equal(value3, result3);
    }

    [Fact]
    public async Task RemoveAsync_SingleValue_ReturnsNull()
    {
        // Arrange
        const long value = 1;

        // Act
        await _tree.InsertAsync(value);
        await _tree.RemoveAsync(value);
        var result = await _tree.GetAsync(value);

        // Assert
        Assert.Equal(default, result);
    }

    [Fact]
    public async Task RemoveAsync_SingleValue_PreservesOtherValues()
    {
        // Arrange
        const long value1 = 1;
        const long value2 = 2;
        const long value3 = 3;

        // Act
        await _tree.InsertAsync(value1);
        await _tree.InsertAsync(value2);
        await _tree.InsertAsync(value3);
        await _tree.RemoveAsync(value2);
        var result1 = await _tree.GetAsync(value1);
        var result2 = await _tree.GetAsync(value2);
        var result3 = await _tree.GetAsync(value3);

        // Assert
        Assert.Equal(value1, result1);
        Assert.Equal(default, result2);
        Assert.Equal(value3, result3);
    }
}
