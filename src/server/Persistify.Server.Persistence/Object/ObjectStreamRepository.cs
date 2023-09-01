using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Abstractions;
using Persistify.Server.Persistence.Exceptions;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Persistence.Object;

public class ObjectStreamRepository<TValue> : IRefTypeStreamRepository<TValue>, IDisposable
    where TValue : class
{
    private readonly Stream _mainStream;
    private readonly ISerializer _serializer;
    private readonly int _sectorSize;
    private readonly IntPairStreamRepository _offsetLengthRepository;
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
        _offsetLengthRepository =
            new IntPairStreamRepository(offsetLengthStream ??
                                        throw new ArgumentNullException(nameof(offsetLengthStream)));
        _buffer = new byte[_sectorSize];
    }

    public async ValueTask<TValue?> ReadAsync(int key)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        var offsetLength = await _offsetLengthRepository.ReadAsync(key);
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
            throw new LowLevelRepositoryCorruptedException();
        }

        return _serializer.Deserialize<TValue>(_buffer.AsMemory()[..length]);
    }

    public async ValueTask<Dictionary<int, TValue>> ReadAllAsync()
    {
        var result = new Dictionary<int, TValue>();
        var ids = await _offsetLengthRepository.ReadAllAsync();

        foreach (var (id, (offset, length)) in ids)
        {
            var byteOffset = SectorsToBytes(offset);
            _mainStream.Seek(byteOffset, SeekOrigin.Begin);
            EnsureBufferCapacity(length);
            var read = await _mainStream.ReadAsync(_buffer, 0, length);
            if (read != length)
            {
                throw new LowLevelRepositoryCorruptedException();
            }

            result.Add(id, _serializer.Deserialize<TValue>(_buffer.AsMemory()[..length]));
        }

        return result;
    }

    public async ValueTask WriteAsync(int key, TValue value)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var currentOffsetLength = await _offsetLengthRepository.ReadAsync(key);

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

    private async ValueTask WriteAtEnd(int id, ReadOnlyMemory<byte> serialized)
    {
        _mainStream.Seek(0, SeekOrigin.End);

        var offset = BytesToSectors(_mainStream.Position);
        var length = serialized.Length;

        await _offsetLengthRepository.WriteAsync(id, (offset, length));
        await _mainStream.WriteAsync(serialized);

        EnsureStreamCorrectLength(_mainStream.Position);
    }

    private async ValueTask WriteAtOffset(int id, int offset, ReadOnlyMemory<byte> serialized, bool isLast)
    {
        var byteOffset = SectorsToBytes(offset);
        _mainStream.Seek(byteOffset, SeekOrigin.Begin);

        await _mainStream.WriteAsync(serialized);
        await _offsetLengthRepository.WriteAsync(id, (offset, serialized.Length));

        if (isLast)
        {
            EnsureStreamCorrectLength(byteOffset + serialized.Length);
        }
    }

    public async ValueTask<bool> DeleteAsync(int key)
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        var offsetLength = await _offsetLengthRepository.ReadAsync(key);
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

        await _offsetLengthRepository.DeleteAsync(key);
        return true;
    }

    public void Clear()
    {
        _mainStream.SetLength(0);
        _offsetLengthRepository.Clear();
    }

    private long SectorsToBytes(int sectors) => sectors * (long)_sectorSize;

    private int BytesToSectors(long bytes)
    {
        int fullSectors = (int)(bytes / _sectorSize);
        return bytes % _sectorSize > 0 ? fullSectors + 1 : fullSectors;
    }

    private void EnsureStreamCorrectLength(long writtenStreamLength)
    {
        var desiredStreamLength = SectorsToBytes(BytesToSectors(writtenStreamLength));
        if (_mainStream.Length != desiredStreamLength)
        {
            _mainStream.SetLength(desiredStreamLength);
        }
    }

    public void Dispose()
    {
        _offsetLengthRepository.Dispose();
        _mainStream.Dispose();
    }

    private void EnsureBufferCapacity(int length)
    {
        if (_buffer.Length < length)
        {
            _buffer = new byte[length * 2];
        }
    }
}
