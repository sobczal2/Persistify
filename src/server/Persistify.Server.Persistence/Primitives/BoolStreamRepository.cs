using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.ErrorHandling;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Persistence.Abstractions;

namespace Persistify.Server.Persistence.Primitives;

public class BoolStreamRepository : IValueTypeStreamRepository<bool>, IDisposable
{
    private readonly ByteArrayStreamRepository _innerRepository;
    private readonly SemaphoreSlim _semaphoreSlim;

    public BoolStreamRepository(
        Stream stream
    )
    {
        _innerRepository = new ByteArrayStreamRepository(stream, sizeof(byte));
        _semaphoreSlim = new SemaphoreSlim(1, 1);
    }

    public void Dispose()
    {
        _innerRepository.Dispose();
        _semaphoreSlim.Dispose();
    }

    public async ValueTask<bool> ReadAsync(int key, bool useLock)
    {
        return useLock ? await _semaphoreSlim.WrapAsync(() => ReadInternalAsync(key)) : await ReadInternalAsync(key);
    }

    public async ValueTask<List<(int key, bool value)>> ReadRangeAsync(int take, int skip, bool useLock)
    {
        return useLock
            ? await _semaphoreSlim.WrapAsync(() => ReadRangeAsync(take, skip))
            : await ReadRangeAsync(take, skip);
    }

    public async ValueTask<int> CountAsync(bool useLock)
    {
        return useLock ? await _semaphoreSlim.WrapAsync(CountInternalAsync) : await CountInternalAsync();
    }

    public async ValueTask WriteAsync(int key, bool value, bool useLock)
    {
        await (useLock
            ? _semaphoreSlim.WrapAsync(() => WriteInternalAsync(key, value))
            : WriteInternalAsync(key, value));
    }

    public async ValueTask<bool> DeleteAsync(int key, bool useLock)
    {
        return useLock
            ? await _semaphoreSlim.WrapAsync(() => DeleteInternalAsync(key))
            : await DeleteInternalAsync(key);
    }

    public void Clear(bool useLock)
    {
        if (useLock)
        {
            _semaphoreSlim.Wrap(ClearInternal);
        }
        else
        {
            ClearInternal();
        }
    }

    public bool IsValueEmpty(bool value)
    {
        return value == EmptyValue;
    }

    public bool EmptyValue => false;

    private async ValueTask<bool> ReadInternalAsync(int key)
    {
        var readBuffer = await _innerRepository.ReadAsync(key / 8, false);

        if (_innerRepository.IsValueEmpty(readBuffer))
        {
            return false;
        }

        return GetValueFromByte(readBuffer[0], key);
    }

    private async ValueTask<List<(int key, bool value)>> ReadRangeAsync(int take, int skip)
    {
        var values = await _innerRepository.ReadRangeAsync(take, skip, false);
        var result = new List<(int key, bool value)>();

        for (var i = 0; i < values.Count; i++)
        {
            var (key, value) = values[i];
            for (var j = 0; j < 8; j++)
            {
                var realKey = (key * 8) + j;
                var realValue = GetValueFromByte(value[0], realKey);

                if (realValue == false)
                {
                    continue;
                }

                if (skip != 0)
                {
                    skip--;
                }

                result.Add((realKey, realValue));
            }
        }

        return result;
    }

    private async ValueTask<int> CountInternalAsync()
    {
        var values = await _innerRepository.ReadRangeAsync(int.MaxValue, 0, false);
        var result = 0;

        for (var i = 0; i < values.Count; i++)
        {
            var (key, value) = values[i];
            for (var j = 0; j < 8; j++)
            {
                var realKey = (key * 8) + j;
                var realValue = GetValueFromByte(value[0], realKey);

                if (realValue == false)
                {
                    continue;
                }

                result++;
            }
        }

        return result;
    }

    private async ValueTask WriteInternalAsync(int key, bool value)
    {
        var byteArr = await _innerRepository.ReadAsync(key / 8, false);

        var buffer = (byte)0x0;

        if (!_innerRepository.IsValueEmpty(byteArr))
        {
            buffer = byteArr[0];
        }

        buffer = SetValueInByte(value, buffer, key);

        await _innerRepository.WriteAsync(key / 8, new[] { buffer }, false);
    }

    private async ValueTask<bool> DeleteInternalAsync(int key)
    {
        var existingValue = await ReadInternalAsync(key);
        if (existingValue == false)
        {
            return false;
        }

        await WriteInternalAsync(key, false);

        return true;
    }

    private void ClearInternal()
    {
        _innerRepository.Clear(false);
    }

    private static bool GetValueFromByte(byte buffer, int key)
    {
        var offset = key % 8;
        buffer >>= offset;
        buffer &= 0x1;
        return buffer switch
        {
            0x0 => false,
            0x1 => true,
            _ => throw new InternalPersistifyException()
        };
    }

    private static byte SetValueInByte(bool value, byte buffer, int key)
    {
        var offset = key % 8;
        var bufferToWrite = value switch
        {
            false => (byte)0x0,
            true => (byte)0x1
        };

        bufferToWrite <<= offset;
        var mask = 0x1 << offset;
        mask = ~mask;
        buffer &= (byte)mask;
        buffer |= bufferToWrite;

        return buffer;
    }
}
