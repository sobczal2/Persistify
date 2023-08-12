using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Helpers.Locking;
using Persistify.Server.Persistence.LowLevel.Abstractions;
using Persistify.Server.Persistence.LowLevel.Exceptions;

namespace Persistify.Server.Persistence.LowLevel.Primitives;

public class LowLevelByteArrayStreamRepository : ILowLevelStreamRepository<byte[]>, IDisposable
{
    private readonly Stream _stream;
    private readonly SemaphoreSlim _semaphore;
    private readonly int _bufferSize;
    private readonly byte[] _buffer;
    private readonly byte[] _emptyValue;

    public LowLevelByteArrayStreamRepository(
        Stream stream,
        int size
    )
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _semaphore = new SemaphoreSlim(1, 1);
        _bufferSize = size;
        _buffer = new byte[_bufferSize];
        _emptyValue = new byte[_bufferSize];
        _emptyValue.AsSpan().Fill(0xFF);
    }

    public async ValueTask<byte[]> ReadAsync(int key, bool useLock = true)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock ? await _semaphore.WrapAsync(() => ReadWithoutLockAsync(key)) : await ReadWithoutLockAsync(key);
    }

    private async ValueTask<byte[]> ReadWithoutLockAsync(int key)
    {
        if (key * (long)_bufferSize >= _stream.Length)
        {
            var value = new byte[_bufferSize];
            value.AsSpan().Fill(0xFF);
            return value;
        }

        _stream.Seek(key * (long)_bufferSize, SeekOrigin.Begin);
        var readBytes = await _stream.ReadAsync(_buffer, 0, _bufferSize);
        if (readBytes != _bufferSize)
        {
            throw new InvalidOperationException();
        }

        if (IsValueEmpty(_buffer))
        {
            var value = new byte[_bufferSize];
            value.AsSpan().Fill(0xFF);
            return value;
        }

        var result = new byte[_bufferSize];
        _buffer.AsSpan().CopyTo(result);
        return result;
    }

    public async ValueTask<Dictionary<int, byte[]>> ReadAllAsync(bool useLock = true)
    {
        return useLock ? await _semaphore.WrapAsync(ReadAllWithoutLockAsync) : await ReadAllWithoutLockAsync();
    }

    private async ValueTask<Dictionary<int, byte[]>> ReadAllWithoutLockAsync()
    {
        var length = _stream.Length;
        _stream.Seek(0, SeekOrigin.Begin);
        var result = new Dictionary<int, byte[]>((int)(length / _bufferSize));
        for (var i = 0; i < length; i += _bufferSize)
        {
            var readBytes = await _stream.ReadAsync(_buffer, 0, _bufferSize);
            if (readBytes != _bufferSize)
            {
                throw new InvalidOperationException();
            }

            if (!IsValueEmpty(_buffer))
            {
                var value = new byte[_bufferSize];
                _buffer.AsSpan().CopyTo(value);
                result.Add(i / _bufferSize, value);
            }
        }

        return result;
    }

    public async ValueTask WriteAsync(int key, byte[] value, bool useLock = true)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        if (value.Length != _bufferSize)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        if (IsValueEmpty(value))
        {
            throw new ArgumentException(nameof(value));
        }

        await (useLock
            ? _semaphore.WrapAsync(() => WriteWithoutLockAsync(key, value))
            : WriteWithoutLockAsync(key, value));
    }

    private async ValueTask WriteWithoutLockAsync(int key, byte[] value)
    {
        var length = _stream.Length;
        if (key * (long)_bufferSize > length)
        {
            _stream.Seek(0, SeekOrigin.End);
            var bytesToWrite = new byte[(key - length / _bufferSize) * _bufferSize];
            bytesToWrite.AsSpan().Fill(0xFF);
            await _stream.WriteAsync(bytesToWrite);
        }
        else
        {
            _stream.Seek(key * (long)_bufferSize, SeekOrigin.Begin);
        }

        await _stream.WriteAsync(value);
        await _stream.FlushAsync();
    }

    public async ValueTask DeleteAsync(int key, bool useLock = true)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        await (useLock
            ? _semaphore.WrapAsync(() => DeleteWithoutLockAsync(key))
            : DeleteWithoutLockAsync(key));
    }

    private async ValueTask DeleteWithoutLockAsync(int key)
    {
        var length = _stream.Length;
        if (key * (long)_bufferSize >= length)
        {
            throw new InvalidOperationException();
        }

        _stream.Seek(key * (long)_bufferSize, SeekOrigin.Begin);
        var readBytes = await _stream.ReadAsync(_buffer, 0, _bufferSize);
        if (readBytes != _bufferSize)
        {
            throw new InvalidOperationException();
        }

        if (IsValueEmpty(_buffer))
        {
            throw new InvalidOperationException();
        }

        if ((key + 1) * (long)_bufferSize == length)
        {
            _stream.SetLength(length - _bufferSize);
        }
        else
        {
            _stream.Seek(key * (long)_bufferSize, SeekOrigin.Begin);
            await _stream.WriteAsync(_emptyValue);
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

    public bool IsValueEmpty(byte[] value)
    {
        return value.AsSpan().SequenceEqual(_emptyValue);
    }

    public byte[] EmptyValue
    {
        get
        {
            var value = new byte[_bufferSize];
            _emptyValue.AsSpan().CopyTo(value);
            return value;
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
