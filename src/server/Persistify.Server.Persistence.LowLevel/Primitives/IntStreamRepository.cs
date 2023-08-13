using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Persistify.Server.Persistence.LowLevel.Abstractions;

namespace Persistify.Server.Persistence.LowLevel.Primitives;

public class IntStreamRepository : IValueTypeStreamRepository<int>, IDisposable
{
    private readonly ByteArrayStreamRepository _innerRepository;

    public IntStreamRepository(
        Stream stream
        )
    {
        _innerRepository = new ByteArrayStreamRepository(stream, sizeof(int));
    }

    public async ValueTask<int> ReadAsync(int key)
    {
        var value = await _innerRepository.ReadAsync(key);
        return BitConverter.ToInt32(value);
    }

    public async ValueTask<Dictionary<int, int>> ReadAllAsync()
    {
        var values = await _innerRepository.ReadAllAsync();
        var result = new Dictionary<int, int>(values.Count);
        foreach (var item in values)
        {
            result[item.Key] = MemoryMarshal.Read<int>(item.Value);
        }

        return result;
    }

    public async ValueTask WriteAsync(int key, int value)
    {
        await _innerRepository.WriteAsync(key, BitConverter.GetBytes(value));
    }

    public async ValueTask<bool> DeleteAsync(int key)
    {
        return await _innerRepository.DeleteAsync(key);
    }

    public void Clear()
    {
        _innerRepository.Clear();
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
