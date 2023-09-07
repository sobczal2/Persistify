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

    public async ValueTask<(int, int)> ReadAsync(int key, bool useLock)
    {
        var value = await _innerRepository.ReadAsync(key, useLock);
        return (
            MemoryMarshal.Read<int>(value.AsSpan(0, sizeof(int))),
            MemoryMarshal.Read<int>(value.AsSpan(sizeof(int), sizeof(int)))
        );
    }

    public async ValueTask<List<(int key, (int, int) value)>> ReadRangeAsync(int take, int skip, bool useLock)
    {
        var values = await _innerRepository.ReadRangeAsync(take, skip, useLock);
        var result = new List<(int key, (int, int) value)>(values.Count);

        foreach (var (key, value) in values)
        {
            result.Add((
                key,
                (
                    MemoryMarshal.Read<int>(value.AsSpan(0, sizeof(int))),
                    MemoryMarshal.Read<int>(value.AsSpan(sizeof(int), sizeof(int)))
                )
            ));
        }

        return result;
    }

    public async ValueTask<int> CountAsync(bool useLock)
    {
        return await _innerRepository.CountAsync(useLock);
    }

    public async ValueTask WriteAsync(int key, (int, int) value, bool useLock)
    {
        MemoryMarshal.Write(_buffer.AsSpan(0, sizeof(int)), ref value.Item1);
        MemoryMarshal.Write(_buffer.AsSpan(sizeof(int), sizeof(int)), ref value.Item2);
        await _innerRepository.WriteAsync(key, _buffer, useLock);
    }

    public async ValueTask<bool> DeleteAsync(int key, bool useLock)
    {
        return await _innerRepository.DeleteAsync(key, useLock);
    }

    public void Clear(bool useLock)
    {
        _innerRepository.Clear(useLock);
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

        GC.SuppressFinalize(this);
    }
}
