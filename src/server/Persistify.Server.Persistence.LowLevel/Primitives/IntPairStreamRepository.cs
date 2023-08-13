using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Helpers.Locking;
using Persistify.Server.Persistence.LowLevel.Abstractions;

namespace Persistify.Server.Persistence.LowLevel.Primitives;

public class IntPairStreamRepository : IValueTypeStreamRepository<(int, int)>, IDisposable
{
    private readonly ByteArrayStreamRepository _innerRepository;
    private readonly byte[] _buffer;
    private readonly SemaphoreSlim _semaphore;

    public IntPairStreamRepository(
        Stream stream
    )
    {
        _innerRepository = new ByteArrayStreamRepository(stream, sizeof(int) * 2);
        _buffer = new byte[sizeof(int) * 2];
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async ValueTask<(int, int)> ReadAsync(int key, bool useLock = true)
    {
        return useLock ? await _semaphore.WrapAsync(() => ReadWithoutLockAsync(key)) : await ReadWithoutLockAsync(key);
    }

    private async ValueTask<(int, int)> ReadWithoutLockAsync(int key)
    {
        var value = await _innerRepository.ReadAsync(key, false);
        return (
            MemoryMarshal.Read<int>(value.AsSpan(0, sizeof(int))),
            MemoryMarshal.Read<int>(value.AsSpan(sizeof(int), sizeof(int)))
        );
    }

    public async ValueTask<Dictionary<int, (int, int)>> ReadAllAsync(bool useLock = true)
    {
        return useLock ? await _semaphore.WrapAsync(ReadAllWithoutLockAsync) : await ReadAllWithoutLockAsync();
    }

    private async ValueTask<Dictionary<int, (int, int)>> ReadAllWithoutLockAsync()
    {
        var values = await _innerRepository.ReadAllAsync(false);
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

    public async ValueTask WriteAsync(int key, (int, int) value, bool useLock = true)
    {
        await (useLock
            ? _semaphore.WrapAsync(() => WriteWithoutLockAsync(key, value))
            : WriteWithoutLockAsync(key, value));
    }

    private async ValueTask WriteWithoutLockAsync(int key, (int, int) value)
    {
        MemoryMarshal.Write(_buffer.AsSpan(0, sizeof(int)), ref value.Item1);
        MemoryMarshal.Write(_buffer.AsSpan(sizeof(int), sizeof(int)), ref value.Item2);
        await _innerRepository.WriteAsync(key, _buffer, false);
    }

    public async ValueTask<bool> DeleteAsync(int key, bool useLock = true)
    {
        return await (useLock
            ? _semaphore.WrapAsync(() => DeleteWithoutLockAsync(key))
            : DeleteWithoutLockAsync(key));
    }

    private async ValueTask<bool> DeleteWithoutLockAsync(int key)
    {
        return await _innerRepository.DeleteAsync(key, false);
    }

    public void Clear(bool useLock = true)
    {
        if (useLock)
        {
            _semaphore.Wait();
            try
            {
                _innerRepository.Clear(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        else
        {
            _innerRepository.Clear(false);
        }
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
        _semaphore.Dispose();
    }
}
