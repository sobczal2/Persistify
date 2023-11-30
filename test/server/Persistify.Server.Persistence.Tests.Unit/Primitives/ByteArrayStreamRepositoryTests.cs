using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Server.Persistence.Primitives;
using Xunit;

namespace Persistify.Server.Persistence.Tests.Unit.Primitives;

public class ByteArrayStreamRepositoryTests : IDisposable
{
    private readonly Stream _stream;
    private readonly ByteArrayStreamRepository _sut;

    public ByteArrayStreamRepositoryTests()
    {
        _stream = new MemoryStream();
        _sut = new ByteArrayStreamRepository(_stream, 1);
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
            var unused = new ByteArrayStreamRepository(stream, 1);
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenSizeIsLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var size = 0;

        // Act
        var action = new Action(() =>
        {
            var unused = new ByteArrayStreamRepository(_stream, size);
        });

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException(
        bool useLock
    )
    {
        // Arrange
        var id = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.ReadAsync(id, useLock));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadAsync_WhenIdIsGreaterThanLength_ReturnsNull(
        bool useLock
    )
    {
        // Arrange
        var id = 1;

        // Act
        var result = await _sut.ReadAsync(id, useLock);

        // Assert
        _sut.IsValueEmpty(result).Should().BeTrue();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadAsync_WhenIdIsLessThanLength_ReturnsValue(
        bool useLock
    )
    {
        // Arrange
        var id = 0;
        var value = new byte[] { 1 };
        await _sut.WriteAsync(id, value, false);

        // Act
        var result = await _sut.ReadAsync(id, useLock);

        // Assert
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadAsync_WhenIdIsOverwritten_ReturnsNewValue(
        bool useLock
    )
    {
        // Arrange
        var id = 0;
        var value = new byte[] { 1 };
        await _sut.WriteAsync(id, value, false);
        var newValue = new byte[] { 2 };
        await _sut.WriteAsync(id, newValue, false);

        // Act
        var result = await _sut.ReadAsync(id, useLock);

        // Assert
        result.Should().BeEquivalentTo(newValue);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadAsync_WhenValueWasDeletedButWasNotLast_ReturnsNull(
        bool useLock
    )
    {
        // Arrange
        var id = 0;
        var value = new byte[] { 1 };
        await _sut.WriteAsync(id, value, false);
        await _sut.DeleteAsync(id, false);
        var id2 = 1;
        var value2 = new byte[] { 2 };
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.ReadAsync(id, useLock);

        // Assert
        _sut.IsValueEmpty(result).Should().BeTrue();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenRepositoryIsEmpty_ReturnsEmptyList(
        bool useLock
    )
    {
        // Arrange

        // Act
        var result = await _sut.ReadRangeAsync(1000, 0, useLock).ToListAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenRepositoryIsNotEmpty_ReturnsList(
        bool useLock
    )
    {
        // Arrange
        var id1 = 0;
        var value1 = new byte[] { 1 };
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = new byte[] { 2 };
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.ReadRangeAsync(1000, 0, useLock).ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.FirstOrDefault(x => x.key == id1).value.Should().BeEquivalentTo(value1);
        result.FirstOrDefault(x => x.key == id2).value.Should().BeEquivalentTo(value2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenTakeIsEqualToZero_ThrowsArgumentOutOfRangeException(
        bool useLock
    )
    {
        // Arrange
        var take = 0;

        // Act
        var action = new Func<Task>(
            async () => await _sut.ReadRangeAsync(take, 0, useLock).ToListAsync()
        );

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenSkipIsLessThanZero_ThrowsArgumentOutOfRangeException(
        bool useLock
    )
    {
        // Arrange
        var skip = -1;

        // Act
        var action = new Func<Task>(
            async () => await _sut.ReadRangeAsync(1000, skip, useLock).ToListAsync()
        );

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenSkipIsEqualToLength_ReturnsEmptyList(
        bool useLock
    )
    {
        // Arrange
        var skip = 1;
        await _sut.WriteAsync(0, new byte[] { 1 }, false);

        // Act
        var result = await _sut.ReadRangeAsync(1000, skip, useLock).ToListAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenSkipIsGreaterThanLength_ReturnsEmptyList(
        bool useLock
    )
    {
        // Arrange
        var skip = 2;
        await _sut.WriteAsync(0, new byte[] { 1 }, false);

        // Act
        var result = await _sut.ReadRangeAsync(1000, skip, useLock).ToListAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenTakeIsGreaterThanLength_ReturnsList(
        bool useLock
    )
    {
        // Arrange
        var take = 2;
        await _sut.WriteAsync(0, new byte[] { 1 }, false);

        // Act
        var result = await _sut.ReadRangeAsync(take, 0, useLock).ToListAsync();

        // Assert
        result.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenTakeIsLessThanLength_ReturnsList(
        bool useLock
    )
    {
        // Arrange
        var take = 1;
        await _sut.WriteAsync(0, new byte[] { 1 }, false);
        await _sut.WriteAsync(1, new byte[] { 2 }, false);

        // Act
        var result = await _sut.ReadRangeAsync(take, 0, useLock).ToListAsync();

        // Assert
        result.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenSkipSkipsOverDeletedValues_ReturnsList(
        bool useLock
    )
    {
        // Arrange
        var skip = 1;
        await _sut.WriteAsync(0, new byte[] { 1 }, false);
        await _sut.DeleteAsync(0, false);
        await _sut.WriteAsync(1, new byte[] { 2 }, false);
        await _sut.WriteAsync(2, new byte[] { 3 }, false);

        // Act
        var result = await _sut.ReadRangeAsync(1000, skip, useLock).ToListAsync();

        // Assert
        result.Should().HaveCount(1);
        result.FirstOrDefault(x => x.key == 2).value.Should().BeEquivalentTo(new byte[] { 3 });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CountAsync_WhenRepositoryIsEmpty_ReturnsZero(
        bool useLock
    )
    {
        // Arrange

        // Act
        var result = await _sut.CountAsync(useLock);

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CountAsync_WhenRepositoryIsNotEmpty_ReturnsCount(
        bool useLock
    )
    {
        // Arrange
        await _sut.WriteAsync(0, new byte[] { 1 }, false);
        await _sut.WriteAsync(1, new byte[] { 2 }, false);

        // Act
        var result = await _sut.CountAsync(useLock);

        // Assert
        result.Should().Be(2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CountAsync_WhenValueIsDeleted_ReturnsCount(
        bool useLock
    )
    {
        // Arrange
        await _sut.WriteAsync(0, new byte[] { 1 }, false);
        await _sut.DeleteAsync(0, false);
        await _sut.WriteAsync(1, new byte[] { 2 }, false);

        // Act
        var result = await _sut.CountAsync(useLock);

        // Assert
        result.Should().Be(1);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException(
        bool useLock
    )
    {
        // Arrange
        var id = -1;
        var value = new byte[] { 1 };

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(id, value, useLock));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenValueIsEqualToEmptyValue_ThrowsArgumentOutOfRangeException(
        bool useLock
    )
    {
        // Arrange
        var id = 0;
        var value = new byte[] { 0xFF };

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(id, value, useLock));

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenIdIsBiggerThanLength_ExtendsStream(
        bool useLock
    )
    {
        // Arrange
        var id = 1;
        var value = new byte[] { 1 };

        // Act
        await _sut.WriteAsync(id, value, useLock);

        // Assert
        _stream.Length.Should().Be((id + 1) * sizeof(byte));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenIdIsBiggerThanLength_DoesNotChangeOtherValues(
        bool useLock
    )
    {
        // Arrange
        var id1 = 0;
        var value1 = new byte[] { 1 };
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = new byte[] { 2 };

        // Act
        await _sut.WriteAsync(id2, value2, useLock);

        // Assert
        var result = await _sut.ReadRangeAsync(1000, 0, false).ToListAsync();
        result.Should().HaveCount(2);
        result.FirstOrDefault(x => x.key == id1).value.Should().BeEquivalentTo(value1);
        result.FirstOrDefault(x => x.key == id2).value.Should().BeEquivalentTo(value2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenValueLengthIsBiggerThanOne_ThrowsArgumentOutOfRangeException(
        bool useLock
    )
    {
        // Arrange
        var id = 0;
        var value = new byte[] { 1, 2 };

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(id, value, useLock));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenValueLengthIsLessThanOne_ThrowsArgumentOutOfRangeException(
        bool useLock
    )
    {
        // Arrange
        var id = 0;
        var value = new byte[] { };

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(id, value, useLock));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenIdIsLessThanZero_ThrowsArgumentOutOfRangeException(
        bool useLock
    )
    {
        // Arrange
        var id = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.DeleteAsync(id, useLock));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenIdIsLessThanLength_DeletesValue(
        bool useLock
    )
    {
        // Arrange
        var id = 0;
        var value = new byte[] { 1 };
        await _sut.WriteAsync(id, value, false);

        // Act
        var result = await _sut.DeleteAsync(id, useLock);

        // Assert
        result.Should().BeTrue();
        var allList = await _sut.ReadRangeAsync(1000, 0, false).ToListAsync();
        allList.Should().BeEmpty();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenIdIsLessThanLength_DoesNotChangeOtherValues(
        bool useLock
    )
    {
        // Arrange
        var id1 = 0;
        var value1 = new byte[] { 1 };
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = new byte[] { 2 };
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.DeleteAsync(id1, useLock);

        // Assert
        result.Should().BeTrue();
        var allList = await _sut.ReadRangeAsync(1000, 0, false).ToListAsync();
        allList.Should().HaveCount(1);
        allList.FirstOrDefault(x => x.key == id2).value.Should().BeEquivalentTo(value2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenIdIsBiggerThanLength_ReturnsFalse(
        bool useLock
    )
    {
        // Arrange
        var id = 1;

        // Act
        var result = await _sut.DeleteAsync(id, useLock);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenIdBelongsToStreamButIsAlreadyDeleted_ReturnsFalse(
        bool useLock
    )
    {
        // Arrange
        var id1 = 0;
        var value1 = new byte[] { 1 };
        await _sut.WriteAsync(id1, value1, false);
        await _sut.DeleteAsync(id1, false);
        var id2 = 1;
        var value2 = new byte[] { 2 };
        await _sut.WriteAsync(id2, value2, false);

        // Act
        var result = await _sut.DeleteAsync(id1, useLock);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenIdIsLastIdAndValueIsDeleted_TruncatesStream(
        bool useLock
    )
    {
        // Arrange
        var id = 0;
        var value = new byte[] { 1 };
        await _sut.WriteAsync(id, value, false);

        // Act
        var result = await _sut.DeleteAsync(id, useLock);

        // Assert
        result.Should().BeTrue();
        _stream.Length.Should().Be(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Clear_WhenCalled_DeletesAllValues(
        bool useLock
    )
    {
        // Arrange
        var id1 = 0;
        var value1 = new byte[] { 1 };
        await _sut.WriteAsync(id1, value1, false);
        var id2 = 1;
        var value2 = new byte[] { 2 };
        await _sut.WriteAsync(id2, value2, false);

        // Act
        _sut.Clear(useLock);

        // Assert
        var result = await _sut.ReadRangeAsync(1000, 0, false).ToListAsync();
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
