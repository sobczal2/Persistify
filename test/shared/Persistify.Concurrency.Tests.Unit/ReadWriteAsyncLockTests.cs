using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Persistify.Concurrency.Tests.Unit;

public class ReadWriteAsyncLockTests
{
    private ReadWriteAsyncLock _sut;
    private static TimeSpan DefaultTimeout => TimeSpan.FromMilliseconds(10);
    private static CancellationToken DefaultCancellationToken => CancellationToken.None;

    public ReadWriteAsyncLockTests()
    {
        _sut = new ReadWriteAsyncLock();
    }

    [Fact]
    public async Task EnterReadLockAsync_WhenNoLocksAreHeld_ReturnsTrue()
    {
        // Arrange
        var id = Guid.Parse("00000000-0000-0000-0000-000000000000");

        // Act
        var result = await _sut.EnterReadLockAsync(id, DefaultTimeout, DefaultCancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EnterReadLockAsync_WhenReadLockIsHeldBySameId_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = Guid.Parse("00000000-0000-0000-0000-000000000000");
        await _sut.EnterReadLockAsync(id, DefaultTimeout, DefaultCancellationToken);

        // Act
        Func<Task> act = async () => await _sut.EnterReadLockAsync(id, DefaultTimeout, DefaultCancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task EnterReadLockAsync_WhenReadLockIsHeldByOtherId_ReturnsTrue()
    {
        // Arrange
        var id1 = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var id2 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        await _sut.EnterReadLockAsync(id1, DefaultTimeout, DefaultCancellationToken);

        // Act
        var result = await _sut.EnterReadLockAsync(id2, DefaultTimeout, DefaultCancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EnterReadLockAsync_WhenWriteLockIsHeldByOtherId_ReturnsFalse()
    {
        // Arrange
        var id1 = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var id2 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        await _sut.EnterWriteLockAsync(id1, DefaultTimeout, DefaultCancellationToken);

        // Act
        var result = await _sut.EnterReadLockAsync(id2, DefaultTimeout, DefaultCancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EnterReadLockAsync_WhenWriteLockIsHeldByOtherIdAndReleased_ReturnsTrue()
    {
        // Arrange
        var id1 = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var id2 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        await _sut.EnterWriteLockAsync(id1, DefaultTimeout, DefaultCancellationToken);
        await _sut.ExitWriteLockAsync(id1);

        // Act
        var result = await _sut.EnterReadLockAsync(id2, DefaultTimeout, DefaultCancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EnterReadLockAsync_WhenReadLockIsHeldByOtherIdAndReleased_ReturnsTrue()
    {
        // Arrange
        var id1 = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var id2 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        await _sut.EnterReadLockAsync(id1, DefaultTimeout, DefaultCancellationToken);
        await _sut.ExitReadLockAsync(id1);

        // Act
        var result = await _sut.EnterReadLockAsync(id2, DefaultTimeout, DefaultCancellationToken);

        // Assert
        result.Should().BeTrue();
    }
}
