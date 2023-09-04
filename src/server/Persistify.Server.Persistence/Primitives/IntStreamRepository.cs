using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Abstractions;

namespace Persistify.Server.Persistence.Primitives;

public class IntStreamRepository : IValueTypeStreamRepository<int>, IDisposable
{
    private readonly ByteArrayStreamRepository _innerRepository;

    public IntStreamRepository(
        Stream stream
        )
    {
        _innerRepository = new ByteArrayStreamRepository(stream, sizeof(int));
    }

    public async ValueTask<int> ReadAsync(int key, bool useLock)
    {
        var value = await _innerRepository.ReadAsync(key, useLock);
        return MemoryMarshal.Read<int>(value);
    }

    public async ValueTask<List<(int key, int value)>> ReadRangeAsync(int take, int skip, bool useLock)
    {
        var values = await _innerRepository.ReadRangeAsync(take, skip, useLock);
        var result = new List<(int key, int value)>(values.Count);

        foreach (var (key, value) in values)
        {
            result.Add((key, MemoryMarshal.Read<int>(value)));
        }

        return result;
    }

    public async ValueTask<int> CountAsync(bool useLock)
    {
        return await _innerRepository.CountAsync(useLock);
    }

    public async ValueTask WriteAsync(int key, int value, bool useLock)
    {
        var bytes = new byte[sizeof(int)];
        MemoryMarshal.Write(bytes, ref value);
        await _innerRepository.WriteAsync(key, bytes, useLock);
    }

    public async ValueTask<bool> DeleteAsync(int key, bool useLock)
    {
        return await _innerRepository.DeleteAsync(key, useLock);
    }

    public void Clear(bool useLock)
    {
        _innerRepository.Clear(useLock);
    }

    public bool IsValueEmpty(int value)
    {
        var bytes = new byte[sizeof(int)];
        MemoryMarshal.Write(bytes, ref value);
        return _innerRepository.IsValueEmpty(bytes);
    }

    public int EmptyValue => MemoryMarshal.Read<int>(_innerRepository.EmptyValue);

    public void Dispose()
    {
        _innerRepository.Dispose();

        GC.SuppressFinalize(this);
    }
}
