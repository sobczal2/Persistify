using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Persistify.Server.Persistence.LowLevel.Abstractions;

namespace Persistify.Server.Persistence.LowLevel.Primitives;

public class LowLevelIntStreamRepository : ILowLevelStreamRepository<int>, IDisposable
{
    private readonly LowLevelByteArrayStreamRepository _innerRepository;

    public LowLevelIntStreamRepository(
        Stream stream
        )
    {
        _innerRepository = new LowLevelByteArrayStreamRepository(stream, sizeof(int));
    }

    public async ValueTask<int> ReadAsync(int key, bool useLock = true)
    {
        var value = await _innerRepository.ReadAsync(key, useLock);
        return BitConverter.ToInt32(value);
    }

    public async ValueTask<Dictionary<int, int>> ReadAllAsync(bool useLock = true)
    {
        var values = await _innerRepository.ReadAllAsync(useLock);
        var result = new Dictionary<int, int>(values.Count);
        foreach (var item in values)
        {
            result[item.Key] = MemoryMarshal.Read<int>(item.Value);
        }

        return result;
    }

    public async ValueTask WriteAsync(int key, int value, bool useLock = true)
    {
        await _innerRepository.WriteAsync(key, BitConverter.GetBytes(value), useLock);
    }

    public async ValueTask DeleteAsync(int key, bool useLock = true)
    {
        await _innerRepository.DeleteAsync(key, useLock);
    }

    public void Clear(bool useLock = true)
    {
        _innerRepository.Clear(useLock);
    }

    public bool IsValueEmpty(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        return _innerRepository.IsValueEmpty(bytes);
    }

    public int EmptyValue => MemoryMarshal.Read<int>(_innerRepository.EmptyValue);

    public void Dispose()
    {
        _innerRepository.Dispose();
    }
}
