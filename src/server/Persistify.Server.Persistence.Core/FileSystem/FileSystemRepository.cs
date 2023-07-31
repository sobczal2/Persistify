using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Exceptions;
using Persistify.Server.Serialization;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemRepository<T> : IRepository<T>, IDisposable, IPurgable
where T : class
{
    private readonly string _mainFilePath;
    private readonly ILongLinearRepository _longLinearRepository;
    private readonly Stream _stream;
    private readonly ISerializer _serializer;
    private readonly SemaphoreSlim _semaphore;
    private const int SectorSize = 1024;
    private byte[] _buffer;

    public FileSystemRepository(
        string mainFilePath,
        ILongLinearRepository longLinearRepository,
        ISerializer serializer
    )
    {
        _mainFilePath = mainFilePath;
        _longLinearRepository = longLinearRepository;
        _stream = new FileStream(mainFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        _serializer = serializer;
        _semaphore = new SemaphoreSlim(1, 1);
        _buffer = new byte[SectorSize];
    }

    private async ValueTask<(int offset, int length)?> GetOffsetAndLength(int id)
    {
        var combined = await _longLinearRepository.ReadAsync(id, false);
        if (combined == null)
        {
            return null;
        }

        return ConvertToOffsetAndLength(combined.Value);
    }
    private async ValueTask SetOffsetAndLength(int id, int offset, int length)
    {
        var combined = ConvertToCombined(offset, length);
        await _longLinearRepository.WriteAsync(id, combined, false);
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
        return offset * SectorSize;
    }

    private int BytesToOffset(long bytes)
    {
        return Math.DivRem((int)bytes, SectorSize, out var remainder) + (remainder > 0 ? 1 : 0);
    }

    private void EnsureBufferCapacity(int length)
    {
        if (_buffer.Length < length)
        {
            _buffer = new byte[length * 2];
        }
    }

    private int CalculateSectorCount(int length)
    {
        return Math.DivRem(length, SectorSize, out var remainder) + (remainder > 0 ? 1 : 0);
    }

    public async ValueTask<T?> ReadAsync(int id)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await _semaphore.WaitAsync();
        try
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
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask<IDictionary<int, T>> ReadAllAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var result = new Dictionary<int, T>();
            var ids = await _longLinearRepository.ReadAllAsync(false);

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
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask WriteAsync(int id, T value)
    {
        if(id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }
        await _semaphore.WaitAsync();
        try
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
                if (CalculateSectorCount(currentLength) >= CalculateSectorCount(serialized.Length))
                {
                    await WriteAtOffset(id, currentOffset, serialized);
                }
                else
                {
                    await WriteAtEnd(id, serialized);
                }
            }
        } catch
        {
            _semaphore.Release();
            throw;
        }
    }

    private async ValueTask WriteAtEnd(int id, ReadOnlyMemory<byte> serialized)
    {
        _stream.Seek(0, SeekOrigin.End);
        var offset = BytesToOffset(_stream.Position);
        var length = serialized.Length;
        await SetOffsetAndLength(id, offset, length);
        await _stream.WriteAsync(serialized);
        await _stream.FlushAsync();
    }

    private async ValueTask WriteAtOffset(int id, int offset, ReadOnlyMemory<byte> serialized)
    {
        var byteOffset = OffsetToBytes(offset);
        _stream.Seek(byteOffset, SeekOrigin.Begin);
        await SetOffsetAndLength(id, offset, serialized.Length);
        await _stream.WriteAsync(serialized);
        await _stream.FlushAsync();
    }

    public async ValueTask<bool> DeleteAsync(int id)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await _semaphore.WaitAsync();
        try
        {
            var offsetAndLength = await GetOffsetAndLength(id);
            if (offsetAndLength == null)
            {
                return false;
            }
            var (offset, length) = offsetAndLength.Value;
            var byteOffset = OffsetToBytes(offset);
            if(byteOffset + length == _stream.Length)
            {
                _stream.SetLength(byteOffset);
            }
            await _longLinearRepository.DeleteAsync(id, false);
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void ClearAsync()
    {
        _semaphore.Wait();
        try
        {
            _stream.SetLength(0);
            _longLinearRepository.ClearAsync(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
        _stream.Dispose();
        if (File.Exists(_mainFilePath) && new FileInfo(_mainFilePath).Length == 0)
        {
            File.Delete(_mainFilePath);
        }
    }

    public async ValueTask PurgeAsync()
    {
        throw new NotImplementedException();
    }
}
