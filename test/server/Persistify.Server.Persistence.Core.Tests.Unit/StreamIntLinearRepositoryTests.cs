using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Server.Persistence.Core.FileSystem;
using Xunit;

namespace Persistify.Server.Persistence.Core.Tests.Unit;

public class StreamIntLinearRepositoryTests
{
    private StreamIntLinearRepository _sut;
    private Stream _stream;

    public StreamIntLinearRepositoryTests()
    {
        _stream = new MemoryStream();
        _sut = new StreamIntLinearRepository(_stream);
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.ReadAsync(id));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsGreaterThanLength_ReturnsNull()
    {
        // Arrange
        var id = 1;

        // Act
        var result = await _sut.ReadAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsLessThanLength_ReturnsValue()
    {
        // Arrange
        var id = 0;
        var value = 1;
        await _sut.WriteAsync(id, value);

        // Act
        var result = await _sut.ReadAsync(id);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsOverwritten_ReturnsNewValue()
    {
        // Arrange
        var id = 0;
        var value = 1;
        await _sut.WriteAsync(id, value);
        var newValue = 2;
        await _sut.WriteAsync(id, newValue);

        // Act
        var result = await _sut.ReadAsync(id);

        // Assert
        result.Should().Be(newValue);
    }

    [Fact]
    public async Task ReadAsync_WhenValueWasDeletedButWasNotLast_ReturnsNull()
    {
        // Arrange
        var id = 0;
        var value = 1;
        await _sut.WriteAsync(id, value);
        await _sut.DeleteAsync(id);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2);

        // Act
        var result = await _sut.ReadAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ReadAllAsync_WhenNoValues_ReturnsEmptyDictionary()
    {
        // Arrange

        // Act
        var result = await _sut.ReadAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadAllAsync_WhenOneValue_ReturnsDictionaryWithOneValue()
    {
        // Arrange
        var id = 0;
        var value = 1;
        await _sut.WriteAsync(id, value);

        // Act
        var result = await _sut.ReadAllAsync();

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
        await _sut.WriteAsync(id1, value1);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2);

        // Act
        var result = await _sut.ReadAllAsync();

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
        await _sut.WriteAsync(id1, value1);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2);
        var newValue2 = 3;
        await _sut.WriteAsync(id2, newValue2);

        // Act
        var result = await _sut.ReadAllAsync();

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
        var action = new Func<Task>(async () => await _sut.WriteAsync(id, value));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task WriteAsync_WhenValueIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = 0;
        var value = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(id, value));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsBiggerThanLength_ExtendsStream()
    {
        // Arrange
        var id = 1;
        var value = 1;

        // Act
        await _sut.WriteAsync(id, value);

        // Assert
        _stream.Length.Should().Be((id + 1) * sizeof(int));
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsBiggerThanLength_DoesNotChangeOtherValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = 1;
        await _sut.WriteAsync(id1, value1);
        var id2 = 1;
        var value2 = 2;

        // Act
        await _sut.WriteAsync(id2, value2);

        // Assert
        var result = await _sut.ReadAllAsync();
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
        var action = new Func<Task>(async () => await _sut.DeleteAsync(id));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsLessThanLength_DeletesValue()
    {
        // Arrange
        var id = 0;
        var value = 1;
        await _sut.WriteAsync(id, value);

        // Act
        await _sut.DeleteAsync(id);

        // Assert
        var result = await _sut.ReadAllAsync();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsLessThanLength_DoesNotChangeOtherValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = 1;
        await _sut.WriteAsync(id1, value1);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2);

        // Act
        await _sut.DeleteAsync(id1);

        // Assert
        var result = await _sut.ReadAllAsync();
        result.Should().HaveCount(1);
        result[id2].Should().Be(value2);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsBiggerThanLength_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = 1;

        // Act
        var action = new Func<Task>(async () => await _sut.DeleteAsync(id));

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdBelongsToStreamButIsAlreadyDeleted_ThrowsInvalidOperationException()
    {
        // Arrange
        var id1 = 0;
        var value1 = 1;
        await _sut.WriteAsync(id1, value1);
        await _sut.DeleteAsync(id1);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2);

        // Act
        var action = new Func<Task>(async () => await _sut.DeleteAsync(id1));

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsLastIdAndValueIsDeleted_TruncatesStream()
    {
        // Arrange
        var id = 0;
        var value = 1;
        await _sut.WriteAsync(id, value);

        // Act
        await _sut.DeleteAsync(id);

        // Assert
        _stream.Length.Should().Be(0);
    }

    [Fact]
    public async Task Clear_WhenCalled_DeletesAllValues()
    {
        // Arrange
        var id1 = 0;
        var value1 = 1;
        await _sut.WriteAsync(id1, value1);
        var id2 = 1;
        var value2 = 2;
        await _sut.WriteAsync(id2, value2);

        // Act
        _sut.Clear();

        // Assert
        var result = await _sut.ReadAllAsync();
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
