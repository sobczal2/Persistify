using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Server.Persistence.Core.Stream;
using Persistify.Server.Serialization;
using Xunit;

namespace Persistify.Server.Persistence.Core.Tests.Unit.Stream;

public class StreamRepositoryTests
{
    public class TestClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private StreamRepository<TestClass> _sut;
    private System.IO.Stream _mainStream;
    private StreamLongLinearRepository _indexRepository;
    private System.IO.Stream _indexStream;
    private int _sectorSize;

    public StreamRepositoryTests()
    {
        _indexStream = new MemoryStream();
        _indexRepository = new StreamLongLinearRepository(_indexStream);
        _mainStream = new MemoryStream();
        _sectorSize = 100;
        _sut = new StreamRepository<TestClass>(
            _indexRepository,
            new JsonSerializer(),
            _mainStream,
            _sectorSize
        );
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
    public async Task ReadAsync_WhenIdIsCorrect_ReturnsValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);

        // Act
        var result = await _sut.ReadAsync(id);

        // Assert
        result.Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsCorrectAndMultipleValues_ReturnsValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        var result = await _sut.ReadAsync(id);

        // Assert
        result.Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task ReadAsync_WhenIdIsCorrectAndMultipleValuesAndIdIsNotFirst_ReturnsValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        var result = await _sut.ReadAsync(id2);

        // Assert
        result.Should().BeEquivalentTo(value2);
    }

    [Fact]
    public async Task ReadAllAsync_WhenRepositoryIsEmpty_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var result = await _sut.ReadAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadAllAsync_WhenRepositoryHasOneValue_ReturnsListWithOneValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);

        // Act
        var result = await _sut.ReadAllAsync();

        // Assert
        result[id].Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task ReadAllAsync_WhenRepositoryHasMultipleValues_ReturnsListWithMultipleValues()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        var result = await _sut.ReadAllAsync();

        // Assert
        result[id].Should().BeEquivalentTo(value);
        result[id2].Should().BeEquivalentTo(value2);
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = -1;
        var value = new TestClass { Id = 1 };

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(id, value));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsCorrect_WritesValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };

        // Act
        await _sut.WriteAsync(id, value);

        // Assert
        var result = await _sut.ReadAsync(id);
        result.Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsCorrectAndMultipleValues_WritesValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };

        // Act
        await _sut.WriteAsync(id2, value2);

        // Assert
        var result = await _sut.ReadAsync(id2);
        result.Should().BeEquivalentTo(value2);
    }

    [Fact]
    public async Task WriteAsync_WhenIdIsCorrectAndMultipleValuesAndIdIsNotFirst_WritesValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        await _sut.WriteAsync(id, value);

        // Assert
        var result = await _sut.ReadAsync(id);
        result.Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task WriteAsync_WhenMultipleValuesAreWritten_ExtendsStreamToLengthDivisibleBySectorSize()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        var result = _mainStream.Length;

        // Assert
        result.Should().Be(2 * _sectorSize);
    }

    [Fact]
    public async Task WriteAsync_WhenOverridingNotLastValueWithLongerValue_WritesValueAtEndOfStream()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);
        var value3 = new TestClass { Id = 3, Name = new string('a', 100) };

        // Act
        await _sut.WriteAsync(id, value3);

        // Assert
        var offset = (int)((await _indexRepository.ReadAsync(id) ?? 0) >> 32);
        offset.Should().Be(2);
    }

    [Fact]
    public async Task WriteAsync_WhenOverridingNotLastValueWithShorterValue_WritesValueAtOffsetOfOverriddenValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1, Name = new string('a', 100) };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);
        var value3 = new TestClass { Id = 3 };

        // Act
        await _sut.WriteAsync(id, value3);

        // Assert
        var offset = (int)((await _indexRepository.ReadAsync(id) ?? 0) >> 32);
        offset.Should().Be(0);
    }

    [Fact]
    public async Task WriteAsync_WhenOverridingLastValueWithLongerValue_WritesValueAtOffsetOfOverriddenValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var value2 = new TestClass { Id = 2, Name = new string('a', 100) };

        // Act
        await _sut.WriteAsync(id, value2);

        // Assert
        var offset = (int)((await _indexRepository.ReadAsync(id) ?? 0) >> 32);
        offset.Should().Be(0);
    }

    [Fact]
    public async Task WriteAsync_WhenOverridingLastValueWithShorterValue_WritesValueAtOffsetOfOverriddenValue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1, Name = new string('a', 100) };
        await _sut.WriteAsync(id, value);
        var value2 = new TestClass { Id = 2 };

        // Act
        await _sut.WriteAsync(id, value2);

        // Assert
        var offset = (int)((await _indexRepository.ReadAsync(id) ?? 0) >> 32);
        offset.Should().Be(0);
    }

    [Fact]
    public async Task WriteAsync_WhenOverridingLastValueWithLongerValue_ExtendsStreamToLengthDivisibleBySectorSize()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var value2 = new TestClass { Id = 2, Name = new string('a', 100) };

        // Act
        await _sut.WriteAsync(id, value2);

        // Assert
        var result = _mainStream.Length;
        result.Should().Be(2 * _sectorSize);
    }

    [Fact]
    public async Task WriteAsync_WhenOverridingLastValueWithShorterValue_ShrinksStreamToLengthDivisibleBySectorSize()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1, Name = new string('a', 100) };
        await _sut.WriteAsync(id, value);
        var value2 = new TestClass { Id = 2 };

        // Act
        await _sut.WriteAsync(id, value2);

        // Assert
        var result = _mainStream.Length;
        result.Should().Be(_sectorSize);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = -1;

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(id);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsGreaterThanMaxValue_ReturnsFalse()
    {
        // Arrange
        var id = 100;

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsCorrectAndValueExists_ReturnsTrue()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsCorrectAndValueDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var id = 0;

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsCorrectAndValueExists_RemovesValueFromIndex()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);

        // Act
        await _sut.DeleteAsync(id);

        // Assert
        var result = await _indexRepository.ReadAsync(id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsCorrectAndValueExists_RemovesValueFromStream()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);

        // Act
        await _sut.DeleteAsync(id);

        // Assert
        var streamLength = _mainStream.Length;
        streamLength.Should().Be(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsCorrectAndMultipleValuesExist_RemovesValueFromIndex()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        await _sut.DeleteAsync(id);

        // Assert
        var result = await _indexRepository.ReadAsync(id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsCorrectAndMultipleValuesExist_ShrinksStreamToLengthDivisibleBySectorSize()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        await _sut.DeleteAsync(id2);

        // Assert
        var streamLength = _mainStream.Length;
        streamLength.Should().Be(_sectorSize);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsCorrect_DoesNotRemoveOtherValuesFromIndex()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        await _sut.DeleteAsync(id);

        // Assert
        var result = await _indexRepository.ReadAsync(id2);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Clear_WhenCalled_DeletesAllValuesFromIndex()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        _sut.Clear();

        // Assert
        var result = await _indexRepository.ReadAsync(id);
        result.Should().BeNull();
        var result2 = await _indexRepository.ReadAsync(id2);
        result2.Should().BeNull();
    }

    [Fact]
    public async Task Clear_WhenCalled_ShrinksStreamToZeroLength()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        _sut.Clear();

        // Assert
        var streamLength = _mainStream.Length;
        streamLength.Should().Be(0);
    }

    [Fact]
    public void Dispose_WhenCalled_DisposesStream()
    {
        // Arrange

        // Act
        _sut.Dispose();

        // Assert
        _mainStream.CanRead.Should().BeFalse();
    }

    [Fact]
    public async Task PurgeAsync_WhenFragmentationExists_CompactsStream()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1, Name = new string('a', 100) };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);
        await _sut.DeleteAsync(id);
        var id3 = 2;
        var value3 = new TestClass { Id = 3 };
        await _sut.WriteAsync(id3, value3);

        // Act
        await _sut.PurgeAsync();

        // Assert
        var streamLength = _mainStream.Length;
        streamLength.Should().Be(2 * _sectorSize);
    }

    [Fact]
    public async Task PurgeAsync_WhenFragmentationExists_DoesNotRemoveValues()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1, Name = new string('a', 100) };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);
        await _sut.DeleteAsync(id);
        var id3 = 2;
        var value3 = new TestClass { Id = 3 };
        await _sut.WriteAsync(id3, value3);

        // Act
        await _sut.PurgeAsync();

        // Assert
        var result = await _sut.ReadAsync(id2);
        result.Should().BeEquivalentTo(value2);
        var result2 = await _sut.ReadAsync(id3);
        result2.Should().BeEquivalentTo(value3);
    }
}
