using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Helpers.Locking;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Exceptions;

namespace Persistify.Server.Persistence.Core.Stream;

public class StreamIntLinearRepository : IIntLinearRepository, IDisposable
{
    private readonly System.IO.Stream _stream;
    private readonly SemaphoreSlim _semaphore;
    private const int EmptyValue = -1;
    private const int BufferSize = sizeof(int);
    private readonly byte[] _buffer;

    public StreamIntLinearRepository(System.IO.Stream stream)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _semaphore = new SemaphoreSlim(1, 1);
        _buffer = new byte[BufferSize];
    }

    public async ValueTask<int?> ReadAsync(int key, bool useLock = true)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock ? await _semaphore.WrapAsync(() => ReadWithoutLockAsync(key)) : await ReadWithoutLockAsync(key);
    }

    private async ValueTask<int?> ReadWithoutLockAsync(int key)
    {
        if (key * (long)BufferSize >= _stream.Length)
        {
            return null;
        }

        _stream.Seek(key * (long)BufferSize, SeekOrigin.Begin);
        var readBytes = await _stream.ReadAsync(_buffer, 0, BufferSize);
        if (readBytes != BufferSize)
        {
            throw new InvalidOperationException();
        }

        var value = BitConverter.ToInt32(_buffer);
        return value == EmptyValue ? null : value;
    }

    public async ValueTask<IDictionary<int, int>> ReadAllAsync(bool useLock = true)
    {
        return useLock ? await _semaphore.WrapAsync(ReadAllWithoutLockAsync) : await ReadAllWithoutLockAsync();
    }

    private async ValueTask<IDictionary<int, int>> ReadAllWithoutLockAsync()
    {
        var length = _stream.Length;
        _stream.Seek(0, SeekOrigin.Begin);
        var result = new Dictionary<int, int>((int)(length / BufferSize));
        for (var i = 0; i < length; i += BufferSize)
        {
            var readBytes = await _stream.ReadAsync(_buffer, 0, BufferSize);
            if (readBytes != BufferSize)
            {
                throw new InvalidOperationException();
            }

            var value = BitConverter.ToInt32(_buffer);
            if (value != EmptyValue)
            {
                result.Add(i / BufferSize, value);
            }
        }

        return result;
    }

    public async ValueTask WriteAsync(int key, int value, bool useLock = true)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        await (useLock
            ? _semaphore.WrapAsync(() => WriteWithoutLockAsync(key, value))
            : WriteWithoutLockAsync(key, value));
    }

    private async ValueTask WriteWithoutLockAsync(int key, int value)
    {
        var length = _stream.Length;
        if (key * (long)BufferSize > length)
        {
            _stream.Seek(0, SeekOrigin.End);
            var bytesToWrite = new byte[(key - length / BufferSize) * BufferSize];
            for (var i = 0; i < bytesToWrite.Length; i += BufferSize)
            {
                if (!BitConverter.TryWriteBytes(bytesToWrite.AsSpan(i), EmptyValue))
                {
                    throw new RepositoryCorruptedException();
                }
            }

            await _stream.WriteAsync(bytesToWrite);
        }
        else
        {
            _stream.Seek(key * (long)BufferSize, SeekOrigin.Begin);
        }

        if (!BitConverter.TryWriteBytes(_buffer, value))
        {
            throw new RepositoryCorruptedException();
        }

        await _stream.WriteAsync(_buffer, 0, BufferSize);
        await _stream.FlushAsync();
    }

    public async ValueTask DeleteAsync(int key, bool useLock = true)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        await (useLock ? _semaphore.WrapAsync(() => DeleteWithoutLockAsync(key)) : DeleteWithoutLockAsync(key));
    }

    private async ValueTask DeleteWithoutLockAsync(int key)
    {
        var length = _stream.Length;
        if (key * (long)BufferSize >= length)
        {
            throw new InvalidOperationException();
        }

        _stream.Seek(key * (long)BufferSize, SeekOrigin.Begin);
        var readBytes = await _stream.ReadAsync(_buffer, 0, BufferSize);
        if (readBytes != BufferSize)
        {
            throw new InvalidOperationException();
        }

        var value = BitConverter.ToInt32(_buffer);
        if (value == EmptyValue)
        {
            throw new InvalidOperationException();
        }

        if ((key + 1) * (long)BufferSize == length)
        {
            _stream.SetLength(length - BufferSize);
        }
        else
        {
            _stream.Seek(key * (long)BufferSize, SeekOrigin.Begin);
            if (!BitConverter.TryWriteBytes(_buffer, EmptyValue))
            {
                throw new RepositoryCorruptedException();
            }

            await _stream.WriteAsync(_buffer, 0, BufferSize);
            await _stream.FlushAsync();
        }
    }

    public void Clear(bool useLock = true)
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
        _stream.SetLength(0);
    }

    public void Dispose()
    {
        _stream.Dispose();
        _semaphore.Dispose();
    }
}
