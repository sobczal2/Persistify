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
    private readonly byte[] _buffer;
    private readonly int _bufferSize;
    private readonly byte[] _emptyValue;
    private readonly SemaphoreSlim _semaphore;
    private readonly Stream _stream;

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

    public void Dispose()
    {
        _stream.Dispose();
        _semaphore.Dispose();

        GC.SuppressFinalize(this);
    }

    public async ValueTask<byte[]> ReadAsync(int key, bool useLock)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock ? await _semaphore.WrapAsync(() => ReadAsyncImpl(key)) : await ReadAsyncImpl(key);
    }

    public async IAsyncEnumerable<(int key, byte[] value)> ReadRangeAsync(int take, int skip, bool useLock)
    {
        if (take <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(take));
        }

        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip));
        }

        if (useLock)
        {
            await _semaphore.WaitAsync();
            try
            {
                await foreach (var result in ReadRangeAsyncImpl(take, skip))
                {
                    yield return result;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            yield break;
        }

        await foreach (var result in ReadRangeAsyncImpl(take, skip))
        {
            yield return result;
        }
    }

    public async ValueTask<int> CountAsync(bool useLock)
    {
        return useLock ? await _semaphore.WrapAsync(CountAsyncImpl) : await CountAsyncImpl();
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

        await (useLock ? _semaphore.WrapAsync(() => WriteAsyncImpl(key, value)) : WriteAsyncImpl(key, value));
    }

    public async ValueTask<bool> DeleteAsync(int key, bool useLock)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock ? await _semaphore.WrapAsync(() => DeleteAsyncImpl(key)) : await DeleteAsyncImpl(key);
    }

    public void Clear(bool useLock)
    {
        if (useLock)
        {
            _semaphore.Wrap(ClearImpl);
        }
        else
        {
            ClearImpl();
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

    private async ValueTask<byte[]> ReadAsyncImpl(int key)
    {
        if (key * (long)_bufferSize >= _stream.Length)
        {
            var value = new byte[_bufferSize];
            value.AsSpan().Fill(0xFF);
            return value;
        }

        _stream.Seek(key * (long)_bufferSize, SeekOrigin.Begin);
        var readBytes = await _stream.ReadAsync(_buffer.AsMemory(0, _bufferSize));
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

    private async IAsyncEnumerable<(int key, byte[] value)> ReadRangeAsyncImpl(int take, int skip)
    {
        var length = _stream.Length / _bufferSize;
        _stream.Seek(0, SeekOrigin.Begin);

        var position = 0;

        for (var i = 0; i < skip && position < length; i++)
        {
            var readBytes = await _stream.ReadAsync(_buffer.AsMemory(0, _bufferSize));
            if (readBytes != _bufferSize)
            {
                throw new InvalidOperationException();
            }

            if (IsValueEmpty(_buffer))
            {
                i--;
            }

            position++;
        }

        for (var i = 0; i < take && position < length; i++)
        {
            var readBytes = await _stream.ReadAsync(_buffer.AsMemory(0, _bufferSize));
            if (readBytes != _bufferSize)
            {
                throw new InvalidOperationException();
            }

            if (!IsValueEmpty(_buffer))
            {
                var value = new byte[_bufferSize];
                _buffer.AsSpan().CopyTo(value);
                yield return (position, value);
            }
            else
            {
                i--;
            }

            position++;
        }
    }

    private async ValueTask<int> CountAsyncImpl()
    {
        var length = _stream.Length / _bufferSize;
        var result = 0;
        _stream.Seek(0, SeekOrigin.Begin);

        for (var i = 0; i < length; i++)
        {
            var readBytes = await _stream.ReadAsync(_buffer.AsMemory(0, _bufferSize));
            if (readBytes != _bufferSize)
            {
                throw new InvalidOperationException();
            }

            if (!IsValueEmpty(_buffer))
            {
                result++;
            }
        }

        return result;
    }

    private async ValueTask WriteAsyncImpl(int key, byte[] value)
    {
        var length = _stream.Length;
        if (key * (long)_bufferSize > length)
        {
            _stream.Seek(0, SeekOrigin.End);
            var bytesToWrite = new byte[(key - (length / _bufferSize)) * _bufferSize];
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

    private async ValueTask<bool> DeleteAsyncImpl(int key)
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

    private void ClearImpl()
    {
        _stream.SetLength(0);
    }
}
