using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Server.Persistence.LowLevel.Streams;
using Xunit;

namespace Persistify.Server.Persistence.LowLevel.Tests.Unit.Streams;

public class SelfManagingFileStreamTests : IDisposable
{
    private SelfManagingFileStream _sut;
    private readonly string _filePath;
    private readonly TimeSpan _idleFileTimeout;

    public SelfManagingFileStreamTests()
    {
        _idleFileTimeout = TimeSpan.FromMilliseconds(50);
        _filePath = Path.GetTempFileName();
        _sut = new SelfManagingFileStream(_idleFileTimeout, _filePath);
    }

    public void Dispose()
    {
        _sut.Dispose();
        File.Delete(_filePath);
    }

    private bool IsFileUsed()
    {
        try
        {
            using var stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            stream.Close();
        }
        catch (IOException)
        {
            return true;
        }

        return false;
    }

    [Fact]
    public void Flush_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        _sut.Flush();

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task Flush_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        _sut.Flush();
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task Flush_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        _sut.Flush();
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        _sut.Flush();
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void Read_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        var unused = _sut.Read(Array.Empty<byte>(), 0, 0);

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task Read_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.Read(Array.Empty<byte>(), 0, 0);
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task Read_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.Read(Array.Empty<byte>(), 0, 0);
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        unused = _sut.Read(Array.Empty<byte>(), 0, 0);
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void Seek_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        _sut.Seek(0, SeekOrigin.Begin);

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task Seek_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        _sut.Seek(0, SeekOrigin.Begin);
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task Seek_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        _sut.Seek(0, SeekOrigin.Begin);
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        _sut.Seek(0, SeekOrigin.Begin);
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void SetLength_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        _sut.SetLength(0);

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task SetLength_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        _sut.SetLength(0);
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task SetLength_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        _sut.Write(Array.Empty<byte>(), 0, 0);
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        _sut.Write(Array.Empty<byte>(), 0, 0);
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void Write_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        _sut.Write(Array.Empty<byte>(), 0, 0);

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task Write_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        _sut.Write(Array.Empty<byte>(), 0, 0);
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task Write_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        _sut.Write(Array.Empty<byte>(), 0, 0);
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        _sut.Write(Array.Empty<byte>(), 0, 0);
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void CanRead_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        var unused = _sut.CanRead;

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task CanRead_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.CanRead;
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task CanRead_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.CanRead;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        unused = _sut.CanRead;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void CanSeek_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        var unused = _sut.CanSeek;

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task CanSeek_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.CanSeek;
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task CanSeek_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.CanSeek;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        unused = _sut.CanSeek;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void CanWrite_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        var unused = _sut.CanWrite;

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task CanWrite_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.CanWrite;
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task CanWrite_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.CanWrite;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        unused = _sut.CanWrite;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void Length_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        var unused = _sut.Length;

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task Length_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.Length;
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task Length_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.Length;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        unused = _sut.Length;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void PositionGet_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        var unused = _sut.Position;

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task PositionGet_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.Position;
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task PositionGet_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        var unused = _sut.Position;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        unused = _sut.Position;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public void PositionSet_WhenCalled_KeepFileOpen()
    {
        // Arrange

        // Act
        _sut.Position = 0;

        // Assert
        IsFileUsed().Should().BeTrue();
    }

    [Fact]
    public async Task PositionSet_WhenCalled_ClosesFileAfterTimeout()
    {
        // Arrange

        // Act
        _sut.Position = 0;
        await Task.Delay(_idleFileTimeout + TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeFalse();
    }

    [Fact]
    public async Task PositionSet_WhenCalled_ProlongsTimeout()
    {
        // Arrange

        // Act
        _sut.Position = 0;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));
        _sut.Position = 1;
        await Task.Delay(_idleFileTimeout - TimeSpan.FromMilliseconds(10));

        // Assert
        IsFileUsed().Should().BeTrue();
    }
}
