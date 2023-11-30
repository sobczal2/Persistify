using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.Persistence.Abstractions;
using Persistify.Server.Persistence.Exceptions;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Persistence.Object;

public class ObjectStreamRepository<TValue> : IRefTypeStreamRepository<TValue>, IDisposable
    where TValue : class
{
    private readonly Stream _mainStream;
    private readonly IntPairStreamRepository _offsetLengthRepository;
    private readonly int _sectorSize;
    private readonly SemaphoreSlim _semaphore;
    private readonly ISerializer _serializer;
    private byte[] _buffer;

    public ObjectStreamRepository(
        Stream mainStream,
        Stream offsetLengthStream,
        ISerializer serializer,
        int sectorSize
    )
    {
        if (sectorSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sectorSize));
        }

        _mainStream = mainStream ?? throw new ArgumentNullException(nameof(mainStream));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _sectorSize = sectorSize;
        _offsetLengthRepository = new IntPairStreamRepository(
            offsetLengthStream ?? throw new ArgumentNullException(nameof(offsetLengthStream))
        );
        _buffer = new byte[_sectorSize];
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public void Dispose()
    {
        _offsetLengthRepository.Dispose();
        _mainStream.Dispose();
        _semaphore.Dispose();

        GC.SuppressFinalize(this);
    }

    public async ValueTask<TValue?> ReadAsync(
        int key,
        bool useLock
    )
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock
            ? await _semaphore.WrapAsync(() => ReadAsyncImpl(key))
            : await ReadAsyncImpl(key);
    }

    public async ValueTask<bool> ExistsAsync(
        int key,
        bool useLock
    )
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock
            ? await _semaphore.WrapAsync(() => ExistsAsyncImpl(key))
            : await ExistsAsyncImpl(key);
    }

    public async IAsyncEnumerable<(int Key, TValue Value)> ReadRangeAsync(
        int take,
        int skip,
        bool useLock
    )
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

    public async ValueTask<int> CountAsync(
        bool useLock
    )
    {
        return useLock ? await _semaphore.WrapAsync(CountAsyncImpl) : await CountAsyncImpl();
    }

    public async ValueTask WriteAsync(
        int key,
        TValue value,
        bool useLock
    )
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        await (
            useLock
                ? _semaphore.WrapAsync(() => WriteAsyncImpl(key, value))
                : WriteAsyncImpl(key, value)
        );
    }

    public async ValueTask<bool> DeleteAsync(
        int key,
        bool useLock
    )
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock
            ? await _semaphore.WrapAsync(() => DeleteAsyncImpl(key))
            : await DeleteAsyncImpl(key);
    }

    public void Clear(
        bool useLock
    )
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

    private async ValueTask<TValue?> ReadAsyncImpl(
        int key
    )
    {
        var offsetLength = await _offsetLengthRepository.ReadAsync(key, false);
        if (_offsetLengthRepository.IsValueEmpty(offsetLength))
        {
            return null;
        }

        var (offset, length) = offsetLength;

        var byteOffset = SectorsToBytes(offset);
        _mainStream.Seek(byteOffset, SeekOrigin.Begin);

        EnsureBufferCapacity(length);

        var read = await _mainStream.ReadAsync(_buffer, 0, length);
        if (read != length)
        {
            throw new RepositoryCorruptedException();
        }

        return _serializer.Deserialize<TValue>(_buffer.AsMemory()[..length]);
    }

    private async ValueTask<bool> ExistsAsyncImpl(
        int key
    )
    {
        var offsetLength = await _offsetLengthRepository.ReadAsync(key, false);
        return !_offsetLengthRepository.IsValueEmpty(offsetLength);
    }

    private async IAsyncEnumerable<(int Key, TValue Value)> ReadRangeAsyncImpl(
        int take,
        int skip
    )
    {
        _mainStream.Seek(0, SeekOrigin.Begin);

        await foreach (
            var (key, offsetLength) in _offsetLengthRepository.ReadRangeAsync(take, skip, false)
        )
        {
            var (offset, length) = offsetLength;
            var byteOffset = SectorsToBytes(offset);
            _mainStream.Seek(byteOffset, SeekOrigin.Begin);

            EnsureBufferCapacity(length);

            var read = await _mainStream.ReadAsync(_buffer.AsMemory(0, length));
            if (read != length)
            {
                throw new RepositoryCorruptedException();
            }

            yield return (key, _serializer.Deserialize<TValue>(_buffer.AsMemory()[..length]));
        }
    }

    private async ValueTask<int> CountAsyncImpl()
    {
        return await _offsetLengthRepository.CountAsync(false);
    }

    private async ValueTask WriteAsyncImpl(
        int key,
        TValue value
    )
    {
        var currentOffsetLength = await _offsetLengthRepository.ReadAsync(key, false);

        var serialized = _serializer.Serialize(value);

        if (_offsetLengthRepository.IsValueEmpty(currentOffsetLength))
        {
            await WriteAtEnd(key, serialized);
        }
        else
        {
            var (currentOffset, currentLength) = currentOffsetLength;
            var byteOffset = SectorsToBytes(currentOffset);
            if (BytesToSectors(byteOffset + currentLength) * _sectorSize == _mainStream.Length)
            {
                await WriteAtOffset(key, currentOffset, serialized, true);
            }
            else if (BytesToSectors(currentLength) >= BytesToSectors(serialized.Length))
            {
                await WriteAtOffset(key, currentOffset, serialized, false);
            }
            else
            {
                await WriteAtEnd(key, serialized);
            }
        }

        await _mainStream.FlushAsync();
    }

    private async ValueTask WriteAtEnd(
        int id,
        ReadOnlyMemory<byte> serialized
    )
    {
        _mainStream.Seek(0, SeekOrigin.End);

        var offset = BytesToSectors(_mainStream.Position);
        var length = serialized.Length;

        await _offsetLengthRepository.WriteAsync(id, (offset, length), false);
        await _mainStream.WriteAsync(serialized);

        EnsureStreamCorrectLength(_mainStream.Position);
    }

    private async ValueTask WriteAtOffset(
        int id,
        int offset,
        ReadOnlyMemory<byte> serialized,
        bool isLast
    )
    {
        var byteOffset = SectorsToBytes(offset);
        _mainStream.Seek(byteOffset, SeekOrigin.Begin);

        await _mainStream.WriteAsync(serialized);
        await _offsetLengthRepository.WriteAsync(id, (offset, serialized.Length), false);

        if (isLast)
        {
            EnsureStreamCorrectLength(byteOffset + serialized.Length);
        }
    }

    private async ValueTask<bool> DeleteAsyncImpl(
        int key
    )
    {
        var offsetLength = await _offsetLengthRepository.ReadAsync(key, false);
        if (_offsetLengthRepository.IsValueEmpty(offsetLength))
        {
            return false;
        }

        var (offset, length) = offsetLength;
        var byteOffset = SectorsToBytes(offset);
        if (SectorsToBytes(BytesToSectors(byteOffset + length)) == _mainStream.Length)
        {
            _mainStream.SetLength(byteOffset);
        }

        if (!await _offsetLengthRepository.DeleteAsync(key, false))
        {
            throw new RepositoryCorruptedException();
        }

        return true;
    }

    private void ClearImpl()
    {
        _mainStream.SetLength(0);
        _offsetLengthRepository.Clear(false);
    }

    private long SectorsToBytes(
        int sectors
    )
    {
        return sectors * (long)_sectorSize;
    }

    private int BytesToSectors(
        long bytes
    )
    {
        var fullSectors = (int)(bytes / _sectorSize);
        return bytes % _sectorSize > 0 ? fullSectors + 1 : fullSectors;
    }

    private void EnsureStreamCorrectLength(
        long writtenStreamLength
    )
    {
        var desiredStreamLength = SectorsToBytes(BytesToSectors(writtenStreamLength));
        if (_mainStream.Length != desiredStreamLength)
        {
            _mainStream.SetLength(desiredStreamLength);
        }
    }

    private void EnsureBufferCapacity(
        int length
    )
    {
        if (_buffer.Length < length)
        {
            _buffer = new byte[length * 2];
        }
    }
}
