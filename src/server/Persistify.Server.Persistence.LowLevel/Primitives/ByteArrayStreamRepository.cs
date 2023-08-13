using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Helpers.Locking;
using Persistify.Server.Persistence.LowLevel.Abstractions;

namespace Persistify.Server.Persistence.LowLevel.Primitives;

public class ByteArrayStreamRepository : IValueTypeStreamRepository<byte[]>, IDisposable
{
    private readonly Stream _stream;
    private readonly int _bufferSize;
    private readonly byte[] _buffer;
    private readonly byte[] _emptyValue;

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
    }

    public async ValueTask<byte[]> ReadAsync(int key)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

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

    public async ValueTask<Dictionary<int, byte[]>> ReadAllAsync()
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

    public async ValueTask WriteAsync(int key, byte[] value)
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

    public async ValueTask<bool> DeleteAsync(int key)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

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

    public void Clear()
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
    }
}
