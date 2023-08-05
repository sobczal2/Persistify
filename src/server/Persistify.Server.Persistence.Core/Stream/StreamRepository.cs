using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Helpers.Locking;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Exceptions;
using Persistify.Server.Serialization;

namespace Persistify.Server.Persistence.Core.Stream;

public class StreamRepository<T> : IRepository<T>, IDisposable, IPurgable
    where T : class
{
    private readonly ILongLinearRepository _indexRepository;
    private readonly System.IO.Stream _stream;
    private readonly ISerializer _serializer;
    private readonly SemaphoreSlim _semaphore;
    private readonly int _sectorSize;
    private byte[] _buffer;

    public StreamRepository(
        ILongLinearRepository indexRepository,
        ISerializer serializer,
        System.IO.Stream stream,
        int sectorSize
    )
    {
        _indexRepository = indexRepository;
        _stream = stream;
        _serializer = serializer;
        _sectorSize = sectorSize;
        _semaphore = new SemaphoreSlim(1, 1);
        _buffer = new byte[_sectorSize];
    }

    private async ValueTask<(int offset, int length)?> GetOffsetAndLength(int id)
    {
        var combined = await _indexRepository.ReadAsync(id, false);
        if (combined == null)
        {
            return null;
        }

        return ConvertToOffsetAndLength(combined.Value);
    }

    private async ValueTask SetOffsetAndLength(int id, int offset, int length)
    {
        var combined = ConvertToCombined(offset, length);
        await _indexRepository.WriteAsync(id, combined, false);
    }

    private (int offset, int length) ConvertToOffsetAndLength(long combined)
    {
        var offset = (int)(combined >> 32);
        var length = (int)(combined & 0xFFFFFFFF);
        return (offset, length);
    }

    private long ConvertToCombined(int offset, int length)
    {
        var combined = ((long)offset << 32) | (uint)length;
        return combined;
    }

    private long OffsetToBytes(int offset)
    {
        return offset * _sectorSize;
    }

    private int BytesToOffset(long bytes)
    {
        return Math.DivRem((int)bytes, _sectorSize, out var remainder) + (remainder > 0 ? 1 : 0);
    }

    private void EnsureBufferCapacity(int length)
    {
        if (_buffer.Length < length)
        {
            _buffer = new byte[length * 2];
        }
    }

    private int CalculateSectorCount(long length)
    {
        return (int)Math.DivRem(length, _sectorSize, out var remainder) + (remainder > 0 ? 1 : 0);
    }

    public async ValueTask<T?> ReadAsync(int id, bool useLock = true)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        return useLock ? await _semaphore.WrapAsync(() => ReadWithoutLockAsync(id)) : await ReadWithoutLockAsync(id);
    }

    private async ValueTask<T?> ReadWithoutLockAsync(int id)
    {
        var offsetAndLength = await GetOffsetAndLength(id);
        if (offsetAndLength == null)
        {
            return null;
        }

        var (offset, length) = offsetAndLength.Value;
        var byteOffset = OffsetToBytes(offset);
        _stream.Seek(byteOffset, SeekOrigin.Begin);
        EnsureBufferCapacity(length);
        var read = await _stream.ReadAsync(_buffer, 0, length);
        if (read != length)
        {
            throw new RepositoryCorruptedException();
        }

        return _serializer.Deserialize<T>(_buffer.AsMemory()[..length]);
    }

    public async ValueTask<IDictionary<int, T>> ReadAllAsync(bool useLock = true)
    {
        return useLock ? await _semaphore.WrapAsync(ReadAllWithoutLockAsync) : await ReadAllWithoutLockAsync();
    }

    private async ValueTask<IDictionary<int, T>> ReadAllWithoutLockAsync()
    {
        var result = new Dictionary<int, T>();
        var ids = await _indexRepository.ReadAllAsync(false);

        foreach (var (id, combined) in ids)
        {
            var (offset, length) = ConvertToOffsetAndLength(combined);
            var byteOffset = OffsetToBytes(offset);
            _stream.Seek(byteOffset, SeekOrigin.Begin);
            EnsureBufferCapacity(length);
            var read = await _stream.ReadAsync(_buffer, 0, length);
            if (read != length)
            {
                throw new RepositoryCorruptedException();
            }

            result.Add(id, _serializer.Deserialize<T>(_buffer.AsMemory()[..length]));
        }

        return result;
    }

    public async ValueTask WriteAsync(int id, T value, bool useLock = true)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (useLock)
        {
            await _semaphore.WrapAsync(() => WriteWithoutLockAsync(id, value));
        }
        else
        {
            await WriteWithoutLockAsync(id, value);
        }
    }

    private async ValueTask WriteWithoutLockAsync(int id, T value)
    {
        var currentOffsetAndLength = await GetOffsetAndLength(id);
        var serialized = _serializer.Serialize(value);
        if (currentOffsetAndLength == null)
        {
            await WriteAtEnd(id, serialized);
        }
        else
        {
            var (currentOffset, currentLength) = currentOffsetAndLength.Value;
            var byteOffset = OffsetToBytes(currentOffset);
            if (CalculateSectorCount(byteOffset + currentLength) * _sectorSize == _stream.Length)
            {
                await WriteAtOffset(id, currentOffset, serialized, true);
            }
            else if (CalculateSectorCount(currentLength) >= CalculateSectorCount(serialized.Length))
            {
                await WriteAtOffset(id, currentOffset, serialized, false);
            }
            else
            {
                await WriteAtEnd(id, serialized);
            }
        }

        await _stream.FlushAsync();
    }

    private async ValueTask WriteAtEnd(int id, ReadOnlyMemory<byte> serialized)
    {
        _stream.Seek(0, SeekOrigin.End);
        var offset = BytesToOffset(_stream.Position);
        var length = serialized.Length;
        await SetOffsetAndLength(id, offset, length);
        await _stream.WriteAsync(serialized);
        EnsureStreamCorrectLength(_stream.Position);
    }

    private async ValueTask WriteAtOffset(int id, int offset, ReadOnlyMemory<byte> serialized, bool isLast)
    {
        var byteOffset = OffsetToBytes(offset);
        _stream.Seek(byteOffset, SeekOrigin.Begin);
        await SetOffsetAndLength(id, offset, serialized.Length);
        await _stream.WriteAsync(serialized);

        if (isLast)
        {
            EnsureStreamCorrectLength(byteOffset + serialized.Length);
        }
    }

    private void EnsureStreamCorrectLength(long writtenStreamLength)
    {
        var desiredStreamLength = CalculateSectorCount(writtenStreamLength) * _sectorSize;
        if (_stream.Length != desiredStreamLength)
        {
            _stream.SetLength(desiredStreamLength);
        }
    }

    public async ValueTask<bool> DeleteAsync(int id, bool useLock = true)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        return useLock
            ? await _semaphore.WrapAsync(() => DeleteWithoutLockAsync(id))
            : await DeleteWithoutLockAsync(id);
    }

    private async ValueTask<bool> DeleteWithoutLockAsync(int id)
    {
        var offsetAndLength = await GetOffsetAndLength(id);
        if (offsetAndLength == null)
        {
            return false;
        }

        var (offset, length) = offsetAndLength.Value;
        var byteOffset = OffsetToBytes(offset);
        if (CalculateSectorCount(byteOffset + length) * _sectorSize == _stream.Length)
        {
            _stream.SetLength(byteOffset);
        }

        await _indexRepository.DeleteAsync(id, false);
        return true;
    }

    public void Clear(bool useLock = true)
    {
        if (useLock)
        {
            _semaphore.Wrap(ClearWithoutLock);
        }
        else
        {
            ClearWithoutLock();
        }
    }

    private void ClearWithoutLock()
    {
        _stream.SetLength(0);
        _indexRepository.Clear(false);
    }

    public void Dispose()
    {
        _semaphore.Dispose();
        _stream.Dispose();
        (_indexRepository as IDisposable)?.Dispose();
    }

    public async ValueTask PurgeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var ids = await _indexRepository.ReadAllAsync(false);
            var idWithOffsetAndLength = new List<(int id, int offset, int length)>();
            foreach (var (id, combined) in ids)
            {
                var (offset, length) = ConvertToOffsetAndLength(combined);
                idWithOffsetAndLength.Add((id, offset, length));
            }

            idWithOffsetAndLength.Sort((a, b) => a.offset.CompareTo(b.offset));
            var newOffset = 0;
            foreach (var (id, offset, length) in idWithOffsetAndLength)
            {
                EnsureBufferCapacity(length);
                var byteOffset = OffsetToBytes(offset);
                _stream.Seek(byteOffset, SeekOrigin.Begin);
                var read = await _stream.ReadAsync(_buffer, 0, length);
                if (read != length)
                {
                    throw new RepositoryCorruptedException();
                }

                var newByteOffset = OffsetToBytes(newOffset);
                _stream.Seek(newByteOffset, SeekOrigin.Begin);
                await _stream.WriteAsync(_buffer.AsMemory()[..length]);
                await SetOffsetAndLength(id, newOffset, length);
                newOffset += BytesToOffset(length);
            }

            _stream.SetLength(OffsetToBytes(newOffset));

            await _stream.FlushAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
