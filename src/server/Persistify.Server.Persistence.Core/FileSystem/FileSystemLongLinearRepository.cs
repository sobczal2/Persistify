using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Helpers.Locking;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Exceptions;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemLongLinearRepository : ILongLinearRepository, IDisposable
{
    private readonly string _filePath;
    private readonly FileStream _fileStream;
    private readonly SemaphoreSlim _semaphore;
    private const long EmptyValue = -1L;
    private const int BufferSize = sizeof(long);
    private readonly byte[] _buffer;

    public FileSystemLongLinearRepository(string filePath)
    {
        _filePath = filePath;
        _fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        _semaphore = new SemaphoreSlim(1, 1);
        _buffer = new byte[BufferSize];
    }

    public async ValueTask<long?> ReadAsync(int id, bool useLock = true)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        return useLock ? await _semaphore.WrapAsync(() => ReadWithoutLockAsync(id)) : await ReadWithoutLockAsync(id);
    }

    private async ValueTask<long?> ReadWithoutLockAsync(int key)
    {
        if (key >= _fileStream.Length / BufferSize)
        {
            return null;
        }

        _fileStream.Seek(key * BufferSize, SeekOrigin.Begin);
        var readBytes = await _fileStream.ReadAsync(_buffer, 0, BufferSize);
        if (readBytes != BufferSize)
        {
            throw new InvalidOperationException();
        }

        return BitConverter.ToInt64(_buffer);
    }

    public async ValueTask<IDictionary<int, long>> ReadAllAsync(bool useLock = true)
    {
        return useLock ? await _semaphore.WrapAsync(ReadAllWithoutLockAsync) : await ReadAllWithoutLockAsync();
    }

    private async ValueTask<IDictionary<int, long>> ReadAllWithoutLockAsync()
    {
        var length = _fileStream.Length;
        _fileStream.Seek(0, SeekOrigin.Begin);
        var result = new Dictionary<int, long>((int)(length / BufferSize));
        for (var i = 0; i < length; i += BufferSize)
        {
            var readBytes = await _fileStream.ReadAsync(_buffer, 0, BufferSize);
            if (readBytes != BufferSize)
            {
                throw new InvalidOperationException();
            }

            var value = BitConverter.ToInt64(_buffer);
            if (value != EmptyValue)
            {
                result.Add(i / BufferSize, value);
            }
        }

        return result;
    }

    public async ValueTask WriteAsync(int id, long value, bool useLock = true)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        await (useLock ? _semaphore.WrapAsync(() => WriteWithoutLockAsync(id, value)) : WriteWithoutLockAsync(id, value));
    }

    private async ValueTask WriteWithoutLockAsync(int key, long value)
    {
        var length = _fileStream.Length;
        if (key >= length / BufferSize)
        {
            _fileStream.Seek(0, SeekOrigin.End);
            var bytesToWrite = new byte[(key - length / BufferSize + 1) * BufferSize];
            for (var i = 0; i < bytesToWrite.Length; i += BufferSize)
            {
                if (!BitConverter.TryWriteBytes(bytesToWrite.AsSpan(i), EmptyValue))
                {
                    throw new RepositoryCorruptedException();
                }
            }

            await _fileStream.WriteAsync(bytesToWrite);
        }
        else
        {
            _fileStream.Seek(key * BufferSize, SeekOrigin.Begin);
        }

        if (!BitConverter.TryWriteBytes(_buffer, value))
        {
            throw new RepositoryCorruptedException();
        }
        await _fileStream.WriteAsync(_buffer, 0, BufferSize);
        await _fileStream.FlushAsync();
    }

    public async ValueTask DeleteAsync(int id, bool useLock = true)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await (useLock ? _semaphore.WrapAsync(() => DeleteWithoutLockAsync(id)) : DeleteWithoutLockAsync(id));
    }

    private async ValueTask DeleteWithoutLockAsync(int key)
    {
        var length = _fileStream.Length;
        if (key >= length / BufferSize)
        {
            throw new InvalidOperationException();
        }

        _fileStream.Seek(key * BufferSize, SeekOrigin.Begin);
        var readBytes = await _fileStream.ReadAsync(_buffer, 0, BufferSize);
        if (readBytes != BufferSize)
        {
            throw new InvalidOperationException();
        }

        var value = BitConverter.ToInt64(_buffer);
        if (value == EmptyValue)
        {
            throw new InvalidOperationException();
        }

        _fileStream.Seek(key * BufferSize, SeekOrigin.Begin);
        if (!BitConverter.TryWriteBytes(_buffer, EmptyValue))
        {
            throw new RepositoryCorruptedException();
        }
        await _fileStream.WriteAsync(_buffer, 0, BufferSize);
        await _fileStream.FlushAsync();
    }

    public void ClearAsync(bool useLock = true)
    {
        if (useLock)
        {
            _semaphore.Wrap(ClearWithoutLockAsync);
        }
        else
        {
            ClearWithoutLockAsync();
        }
    }

    private void ClearWithoutLockAsync()
    {
        _fileStream.SetLength(0);
    }

    public void Dispose()
    {
        _fileStream.Dispose();
        _semaphore.Dispose();
        if (File.Exists(_filePath) && new FileInfo(_filePath).Length == 0)
        {
            File.Delete(_filePath);
        }
    }
}
