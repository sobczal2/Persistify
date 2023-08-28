using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Persistify.Concurrency.Tests.Unit;

public class ReadWriteAsyncLockTests
{
    private ReadWriteAsyncLock _sut;

    public ReadWriteAsyncLockTests()
    {
        _sut = new ReadWriteAsyncLock();
    }

    [Fact]
    public async Task EnterReadLockAsync_WhenNoLocksAreHeld_ReturnsTrue()
    {
        // Arrange
        var id = 1UL;
        var timeout = TimeSpan.FromSeconds(1);

        // Act
        var result = await _sut.EnterReadLockAsync(id, timeout);

        // Assert
        result.Should().BeTrue();
    }
}
