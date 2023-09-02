using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.Persistence.Abstractions;

namespace Persistify.Server.Persistence.Primitives;

public class ByteArrayStreamRepository : IValueTypeStreamRepository<byte[]>, IDisposable
{
    private readonly Stream _stream;
    private readonly int _bufferSize;
    private readonly byte[] _buffer;
    private readonly byte[] _emptyValue;
    private readonly SemaphoreSlim _semaphore;

    public ByteArrayStreamRepository(
        Stream stream,
        int size
    )
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _bufferSize = size;
        _buffer = new byte[_bufferSize];
        _emptyValue = new byte[_bufferSize];
        _emptyValue.AsSpan().Fill(0xFF);
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async ValueTask<byte[]> ReadAsync(int key, bool useLock)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock ? await _semaphore.WrapAsync(() => ReadInternalAsync(key)) : await ReadInternalAsync(key);
    }

    private async ValueTask<byte[]> ReadInternalAsync(int key)
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

    public async ValueTask<Dictionary<int, byte[]>> ReadAllAsync(bool useLock)
    {
        return useLock ? await _semaphore.WrapAsync(ReadAllInternalAsync) : await ReadAllInternalAsync();
    }

    private async ValueTask<Dictionary<int, byte[]>> ReadAllInternalAsync()
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

    public async ValueTask WriteAsync(int key, byte[] value, bool useLock)
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

        await (useLock ? _semaphore.WrapAsync(() => WriteInternalAsync(key, value)) : WriteInternalAsync(key, value));
    }

    private async ValueTask WriteInternalAsync(int key, byte[] value)
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

    public async ValueTask<bool> DeleteAsync(int key, bool useLock)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock ? await _semaphore.WrapAsync(() => DeleteInternalAsync(key)) : await DeleteInternalAsync(key);
    }

    private async ValueTask<bool> DeleteInternalAsync(int key)
    {
        var length = _stream.Length;
        if (key * (long)_bufferSize >= length)
        {
            return false;
        }

        _stream.Seek(key * (long)_bufferSize, SeekOrigin.Begin);
        var readBytes = await _stream.ReadAsync(_buffer, 0, _bufferSize);
        if (readBytes != _bufferSize)
        {
            throw new InvalidOperationException();
        }

        if (IsValueEmpty(_buffer))
        {
            return false;
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

        return true;
    }

    public void Clear(bool useLock)
    {
        if (useLock)
        {
            _semaphore.Wrap(ClearInternal);
        }
        else
        {
            ClearInternal();
        }
    }

    private void ClearInternal()
    {
        _stream.SetLength(0);
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

    public void Dispose()
    {
        _stream.Dispose();
        _semaphore.Dispose();

        GC.SuppressFinalize(this);
    }
}
