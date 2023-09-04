using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Server.Persistence.Primitives;
using Xunit;

namespace Persistify.Server.Persistence.Tests.Unit.Primitives;

public class IntPairStreamRepositoryTests : IDisposable
{
    private IntPairStreamRepository _sut;
    private Stream _stream;

    public IntPairStreamRepositoryTests()
    {
        _stream = new MemoryStream();
        _sut = new IntPairStreamRepository(_stream);
    }

    public void Dispose()
    {
        _sut.Dispose();
        _stream.Dispose();
    }

    [Fact]
    public void Ctor_WhenStreamIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Stream stream = null!;

        // Act
        var action = new Action(() =>
        {
            var unused = new IntPairStreamRepository(stream);
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.ReadAsync(id, false));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsGreaterThanLength_ReturnsNull()
    {
        // Arrange
        var id = 1;

        // Act
        var result = await _sut.ReadAsync(id, false);

        // Assert
        _sut.IsValueEmpty(result).Should().BeTrue();
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsLessThanLength_ReturnsValue()
    {
        // Arrange
        var id = 0;
        var value = (1, 2);
        await _sut.WriteAsync(id, value, false);

        // Act
        var result = await _sut.ReadAsync(id, false);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsOverwritten_ReturnsNewValue()
    {
        // Arrange
        var id = 0;
        var value = (1, 2);
        await _sut.WriteAsync(id, value, false);
        var newValue = (3, 4);
        await _sut.WriteAsync(id, newValue, false);

        // Act
        var result = await _sut.ReadAsync(id, false);

        // Assert
        result.Should().Be(newValue);
    }

    [Fact]
    public async Task ReadAsync_WhenValueWasDeletedButWasNotLast_ReturnsNull()
    {
        // Arrange
        var id = 0;
        var value = (1, 2);
        await _sut.WriteAsync(id, value, false);
        await _sut.DeleteAsync(id, false);
        var id2 = 1;
        var value2 = (3, 4);
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.ReadAsync(id, false);

        // Assert
        _sut.IsValueEmpty(result).Should().BeTrue();
    }

    [Fact]
    public async Task ReadRangeAsync_WhenSkipIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var skip = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.ReadRangeAsync(1000, skip, false));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task ReadRangeAsync_WhenSkipIsEqualToLength_ReturnsEmptyList()
    {
        // Arrange
        var skip = 1;
        await _sut.WriteAsync(0, (1, 2), false);

        // Act
        var result = await _sut.ReadRangeAsync(1000, skip, false);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadRangeAsync_WhenSkipIsGreaterThanLength_ReturnsEmptyList()
    {
        // Arrange
        var skip = 2;
        await _sut.WriteAsync(0, (1, 2), false);

        // Act
        var result = await _sut.ReadRangeAsync(1000, skip, false);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadRangeAsync_WhenTakeIsGreaterThanLength_ReturnsList()
    {
        // Arrange
        var take = 2;
        await _sut.WriteAsync(0, (1, 2), false);

        // Act
        var result = await _sut.ReadRangeAsync(take, 0, false);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task ReadRangeAsync_WhenTakeIsLessThanLength_ReturnsList()
    {
        // Arrange
        var take = 1;
        await _sut.WriteAsync(0, (1, 2), false);
        await _sut.WriteAsync(1, (3, 4), false);

        // Act
        var result = await _sut.ReadRangeAsync(take, 0, false);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task ReadRangeAsync_WhenSkipSkipsOverDeletedValues_ReturnsList()
    {
        // Arrange
        var skip = 1;
        await _sut.WriteAsync(0, (1, 2), false);
        await _sut.DeleteAsync(0, false);
        await _sut.WriteAsync(1, (3, 4), false);
        await _sut.WriteAsync(2, (5, 6), false);

        // Act
        var result = await _sut.ReadRangeAsync(1000, skip, false);

        // Assert
        result.Should().HaveCount(1);
        result.FirstOrDefault(x => x.key == 2).value.Should().Be((5, 6));
    }

    [Fact]
    public async Task CountAsync_WhenRepositoryIsEmpty_ReturnsZero()
    {
        // Arrange

        // Act
        var result = await _sut.CountAsync(false);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task CountAsync_WhenRepositoryIsNotEmpty_ReturnsCount()
    {
        // Arrange
        await _sut.WriteAsync(0, (1, 2), false);
        await _sut.WriteAsync(1, (3, 4), false);

        // Act
        var result = await _sut.CountAsync(false);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task CountAsync_WhenValueIsDeleted_ReturnsCount()
    {
        // Arrange
        await _sut.WriteAsync(0, (1, 2), false);
        await _sut.DeleteAsync(0, false);
        await _sut.WriteAsync(1, (3, 4), false);

        // Act
        var result = await _sut.CountAsync(false);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = -1;
        var value = (1, 2);

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(id, value, false));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task WriteAsync_WhenValueIsEmpty_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = 0;
        var value = _sut.EmptyValue;

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(id, value, false));

        // Assert
        _sut.IsValueEmpty(value).Should().BeTrue();
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsBiggerThanLength_ExtendsStream()
    {
        // Arrange
        var id = 1;
        var value = (1, 2);

        // Act
        await _sut.WriteAsync(id, value, false);

        // Assert
        _stream.Length.Should().Be((id + 1) * sizeof(int) * 2);
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsBiggerThanLength_DoesNotChangeOtherValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = (1, 2);
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = (3, 4);

        // Act
        await _sut.WriteAsync(id2, value2, false);

        // Assert
        var result = await _sut.ReadRangeAsync(1000, 0, false);
        result.Should().HaveCount(2);
        result.FirstOrDefault(x => x.key == id1).value.Should().Be(value1);
        result.FirstOrDefault(x => x.key == id2).value.Should().Be(value2);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.DeleteAsync(id, false));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsLessThanLength_DeletesValue()
    {
        // Arrange
        var id = 0;
        var value = (1, 2);
        await _sut.WriteAsync(id, value, false);

        // Act
        var result = await _sut.DeleteAsync(id, false);

        // Assert
        result.Should().BeTrue();
        var allList = await _sut.ReadRangeAsync(1000, 0, false);
        allList.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsLessThanLength_DoesNotChangeOtherValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = (1, 2);
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = (3, 4);
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.DeleteAsync(id1, false);

        // Assert
        result.Should().BeTrue();
        var allList = await _sut.ReadRangeAsync(1000, 0, false);
        allList.Should().HaveCount(1);
        allList.FirstOrDefault(x => x.key == id2).value.Should().Be(value2);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsBiggerThanLength_ReturnsFalse()
    {
        // Arrange
        var id = 1;

        // Act
        var result = await _sut.DeleteAsync(id, false);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdBelongsToStreamButIsAlreadyDeleted_ReturnsFalse()
    {
        // Arrange
        var id1 = 0;
        var value1 = (1, 2);
        await _sut.WriteAsync(id1, value1, false);
        await _sut.DeleteAsync(id1, false);
        var id2 = 1;
        var value2 = (3, 4);
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.DeleteAsync(id1, false);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsLastIdAndValueIsDeleted_TruncatesStream()
    {
        // Arrange
        var id = 0;
        var value = (1, 2);
        await _sut.WriteAsync(id, value, false);

        // Act
        var result = await _sut.DeleteAsync(id, false);

        // Assert
        result.Should().BeTrue();
        _stream.Length.Should().Be(0);
    }

    [Fact]
    public async Task Clear_WhenCalled_DeletesAllValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = (1, 2);
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = (3, 4);
        await _sut.WriteAsync(id2, value2, false);

        // Act
        _sut.Clear(false);

        // Assert
        var result = await _sut.ReadRangeAsync(1000, 0, false);
        result.Should().BeEmpty();
    }

    [Fact]
    public void Dispose_WhenCalled_DisposesStream()
    {
        // Arrange

        // Act
        _sut.Dispose();

        // Assert
        _stream.CanRead.Should().BeFalse();
    }
}
