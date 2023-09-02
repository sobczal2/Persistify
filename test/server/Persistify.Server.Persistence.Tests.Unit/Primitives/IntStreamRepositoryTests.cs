using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Server.Persistence.Primitives;
using Xunit;

namespace Persistify.Server.Persistence.Tests.Unit.Primitives;

public class IntStreamRepositoryTests : IDisposable
{
    private IntStreamRepository _sut;
    private Stream _stream;

    public IntStreamRepositoryTests()
    {
        _stream = new MemoryStream();
        _sut = new IntStreamRepository(_stream);
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
            var unused = new IntStreamRepository(stream);
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
        var value = 1;
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
        var value = 1;
        await _sut.WriteAsync(id, value, false);
        var newValue = 2;
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
        var value = 1;
        await _sut.WriteAsync(id, value, false);
        await _sut.DeleteAsync(id, false);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.ReadAsync(id, false);

        // Assert
        _sut.IsValueEmpty(result).Should().BeTrue();
    }

    [Fact]
    public async Task ReadAllAsync_WhenNoValues_ReturnsEmptyDictionary()
    {
        // Arrange

        // Act
        var result = await _sut.ReadAllAsync(false);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadAllAsync_WhenOneValue_ReturnsDictionaryWithOneValue()
    {
        // Arrange
        var id = 0;
        var value = 1;
        await _sut.WriteAsync(id, value, false);

        // Act
        var result = await _sut.ReadAllAsync(false);

        // Assert
        result.Should().HaveCount(1);
        result[id].Should().Be(value);
    }

    [Fact]
    public async Task ReadAllAsync_WhenMultipleValues_ReturnsDictionaryWithMultipleValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = 1;
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.ReadAllAsync(false);

        // Assert
        result.Should().HaveCount(2);
        result[id1].Should().Be(value1);
        result[id2].Should().Be(value2);
    }

    [Fact]
    public async Task ReadAllAsync_WhenMultipleValuesAndOneIsOverwritten_ReturnsDictionaryWithMultipleValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = 1;
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2, false);
        var newValue2 = 3;
        await _sut.WriteAsync(id2, newValue2, false);

        // Act
        var result = await _sut.ReadAllAsync(false);

        // Assert
        result.Should().HaveCount(2);
        result[id1].Should().Be(value1);
        result[id2].Should().Be(newValue2);
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = -1;
        var value = 1;

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
        var value = 1;

        // Act
        await _sut.WriteAsync(id, value, false);

        // Assert
        _stream.Length.Should().Be((id + 1) * sizeof(int));
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsBiggerThanLength_DoesNotChangeOtherValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = 1;
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = 2;

        // Act
        await _sut.WriteAsync(id2, value2, false);

        // Assert
        var result = await _sut.ReadAllAsync(false);
        result.Should().HaveCount(2);
        result[id1].Should().Be(value1);
        result[id2].Should().Be(value2);
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
        var value = 1;
        await _sut.WriteAsync(id, value, false);

        // Act
        var result = await _sut.DeleteAsync(id, false);

        // Assert
        result.Should().BeTrue();
        var allDictionary = await _sut.ReadAllAsync(false);
        allDictionary.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsLessThanLength_DoesNotChangeOtherValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = 1;
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.DeleteAsync(id1, false);

        // Assert
        result.Should().BeTrue();
        var allDictionary = await _sut.ReadAllAsync(false);
        allDictionary.Should().HaveCount(1);
        allDictionary[id2].Should().Be(value2);
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
        var value1 = 1;
        await _sut.WriteAsync(id1, value1, false);
        await _sut.DeleteAsync(id1, false);
        var id2 = 1;
        var value2 = 2;
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
        var value = 1;
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
        var value1 = 1;
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2, false);

        // Act
        _sut.Clear(false);

        // Assert
        var result = await _sut.ReadAllAsync(false);
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
