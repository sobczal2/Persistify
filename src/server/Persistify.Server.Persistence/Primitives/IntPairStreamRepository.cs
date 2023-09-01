using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Abstractions;

namespace Persistify.Server.Persistence.Primitives;

public class IntPairStreamRepository : IValueTypeStreamRepository<(int, int)>, IDisposable
{
    private readonly ByteArrayStreamRepository _innerRepository;
    private readonly byte[] _buffer;

    public IntPairStreamRepository(
        Stream stream
    )
    {
        _innerRepository = new ByteArrayStreamRepository(stream, sizeof(int) * 2);
        _buffer = new byte[sizeof(int) * 2];
    }

    public async ValueTask<(int, int)> ReadAsync(int key)
    {
        var value = await _innerRepository.ReadAsync(key);
        return (
            MemoryMarshal.Read<int>(value.AsSpan(0, sizeof(int))),
            MemoryMarshal.Read<int>(value.AsSpan(sizeof(int), sizeof(int)))
        );
    }

    public async ValueTask<Dictionary<int, (int, int)>> ReadAllAsync()
    {
        var values = await _innerRepository.ReadAllAsync();
        var result = new Dictionary<int, (int, int)>(values.Count);
        foreach (var item in values)
        {
            result[item.Key] = (
                MemoryMarshal.Read<int>(item.Value.AsSpan(0, sizeof(int))),
                MemoryMarshal.Read<int>(item.Value.AsSpan(sizeof(int), sizeof(int)))
            );
        }

        return result;
    }

    public async ValueTask WriteAsync(int key, (int, int) value)
    {
        MemoryMarshal.Write(_buffer.AsSpan(0, sizeof(int)), ref value.Item1);
        MemoryMarshal.Write(_buffer.AsSpan(sizeof(int), sizeof(int)), ref value.Item2);
        await _innerRepository.WriteAsync(key, _buffer);
    }

    private async ValueTask WriteWithoutLockAsync(int key, (int, int) value)
    {
        MemoryMarshal.Write(_buffer.AsSpan(0, sizeof(int)), ref value.Item1);
        MemoryMarshal.Write(_buffer.AsSpan(sizeof(int), sizeof(int)), ref value.Item2);
        await _innerRepository.WriteAsync(key, _buffer);
    }

    public async ValueTask<bool> DeleteAsync(int key)
    {
        return await _innerRepository.DeleteAsync(key);
    }

    public void Clear()
    {
        _innerRepository.Clear();
    }

    public bool IsValueEmpty((int, int) value)
    {
        var bytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)).ToArray();
        return _innerRepository.IsValueEmpty(bytes);
    }

    public (int, int) EmptyValue => MemoryMarshal.Read<(int, int)>(_innerRepository.EmptyValue.AsSpan());

    public void Dispose()
    {
        _innerRepository.Dispose();
    }
}
