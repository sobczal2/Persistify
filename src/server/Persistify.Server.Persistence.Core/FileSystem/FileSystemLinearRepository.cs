using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemLinearRepository : ILinearRepository, IDisposable
{
    private const long EmptyValue = -1L;
    private readonly FileStream _fileStream;
    private readonly SemaphoreSlim _semaphore;

    private readonly byte[] _buffer;
    private readonly int _bufferSize;

    public FileSystemLinearRepository(string filePath)
    {
        _fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        _semaphore = new SemaphoreSlim(1, 1);
        _bufferSize = sizeof(long);
        _buffer = new byte[_bufferSize];
    }

    public void Dispose()
    {
        _semaphore.Dispose();
        _fileStream.Dispose();

        if(File.Exists(_fileStream.Name) && new FileInfo(_fileStream.Name).Length == 0)
        {
            File.Delete(_fileStream.Name);
        }
    }

    public async ValueTask WriteAsync(long id, long value)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await _semaphore.WaitAsync();
        try
        {
            await WriteInternalAsync(id, value);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask<long?> ReadAsync(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await _semaphore.WaitAsync();
        try
        {
            return await ReadInternalAsync(id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask<IEnumerable<(long Id, long Value)>> ReadAllAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return await ReadAllInternalAsync().ToListAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask RemoveAsync(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await _semaphore.WaitAsync();
        try
        {
            await RemoveInternalAsync(id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public ValueTask<long> CountAsync()
    {
        return ValueTask.FromResult(_fileStream.Length / sizeof(long));
    }

    private async ValueTask WriteInternalAsync(long id, long value)
    {
        if (_fileStream.Length < id * sizeof(long))
        {
            _fileStream.SetLength(id * sizeof(long));
        }

        _fileStream.Position = (id - 1) * sizeof(long);
        BitConverter.TryWriteBytes(_buffer, value);
        await _fileStream.WriteAsync(_buffer, 0, _bufferSize);
        await _fileStream.FlushAsync();
    }

    private async ValueTask<long?> ReadInternalAsync(long id)
    {
        if (_fileStream.Length < id * sizeof(long))
        {
            return null;
        }

        _fileStream.Position = (id - 1) * sizeof(long);
        var read = await _fileStream.ReadAsync(_buffer, 0, _bufferSize);

        if (read != _bufferSize)
        {
            throw new InvalidOperationException();
        }

        var value = BitConverter.ToInt64(_buffer, 0);
        return value == EmptyValue ? null : value;
    }

    private async IAsyncEnumerable<(long, long)> ReadAllInternalAsync()
    {
        _fileStream.Position = 0;
        var buffer = new byte[sizeof(long)];
        for (long id = 1; id <= _fileStream.Length / sizeof(long); id++)
        {
            var read = await _fileStream.ReadAsync(buffer, 0, buffer.Length);

            if (read != buffer.Length)
            {
                throw new InvalidOperationException();
            }

            var value = BitConverter.ToInt64(buffer, 0);
            if (value != EmptyValue)
            {
                yield return (id, value);
            }
        }
    }

    private async ValueTask RemoveInternalAsync(long id)
    {
        if (_fileStream.Length < id * sizeof(long))
        {
            return;
        }

        _fileStream.Position = (id - 1) * sizeof(long);
        var bytes = BitConverter.GetBytes(EmptyValue);
        await _fileStream.WriteAsync(bytes, 0, bytes.Length);
        await _fileStream.FlushAsync();
    }
}
