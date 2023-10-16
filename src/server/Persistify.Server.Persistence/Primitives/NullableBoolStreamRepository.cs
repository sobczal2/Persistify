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

// TODO: delete probably
public class NullableBoolStreamRepository : IValueTypeStreamRepository<bool?>, IDisposable
{
    private const byte NullValueInternal = 0x0;
    private const byte FalseValueInternal = 0x1;
    private const byte TrueValueInternal = 0x2;
    private const byte LocalMaskInternal = 0x3;
    private readonly ByteArrayStreamRepository _innerRepository;
    private readonly SemaphoreSlim _semaphoreSlim;

    public NullableBoolStreamRepository(
        Stream stream
    )
    {
        _innerRepository = new ByteArrayStreamRepository(stream, sizeof(byte));
        _semaphoreSlim = new SemaphoreSlim(1, 1);
    }

    public void Dispose()
    {
        _innerRepository.Dispose();

        GC.SuppressFinalize(this);
    }

    public async ValueTask<bool?> ReadAsync(int key, bool useLock)
    {
        return useLock ? await _semaphoreSlim.WrapAsync(() => ReadInternalAsync(key)) : await ReadInternalAsync(key);
    }

    public async ValueTask<List<(int key, bool? value)>> ReadRangeAsync(int take, int skip, bool useLock)
    {
        if (take <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(take));
        }

        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip));
        }

        return useLock
            ? await _semaphoreSlim.WrapAsync(() => ReadRangeInternalAsync(take, skip))
            : await ReadRangeInternalAsync(take, skip);
    }

    public async ValueTask<int> CountAsync(bool useLock)
    {
        return useLock ? await _semaphoreSlim.WrapAsync(CountInternalAsync) : await CountInternalAsync();
    }

    public async ValueTask WriteAsync(int key, bool? value, bool useLock)
    {
        await (useLock
            ? _semaphoreSlim.WrapAsync(() => WriteInternalAsync(key, value))
            : WriteInternalAsync(key, value));
    }

    public ValueTask<bool> DeleteAsync(int key, bool useLock)
    {
        throw new NotSupportedException("Use WriteAsync with EmptyValue instead");
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

    public bool IsValueEmpty(bool? value)
    {
        return value == EmptyValue;
    }

    public bool? EmptyValue => null;

    private async ValueTask<bool?> ReadInternalAsync(int key)
    {
        var byteArr = await _innerRepository.ReadAsync(key / 4, false);
        if (_innerRepository.IsValueEmpty(byteArr))
        {
            return EmptyValue;
        }

        return GetValueFromByte(byteArr[0], key);
    }

    private async ValueTask<List<(int key, bool? value)>> ReadRangeInternalAsync(int take, int skip)
    {
        var values = await _innerRepository.ReadRangeAsync(int.MaxValue, 0, false);
        var result = new List<(int key, bool? value)>(take);

        for (var i = 0; i < values.Count; i++)
        {
            var (currentKey, currentValue) = values[i];
            currentKey *= 4;
            for (var j = 0; j < 4; j++)
            {
                if (result.Count == take)
                {
                    return result;
                }

                var realKey = currentKey + j;
                var realValue = GetValueFromByte(currentValue[0], realKey);
                if (realValue == EmptyValue)
                {
                    continue;
                }

                if (skip != 0)
                {
                    skip--;
                    continue;
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
            var (currentKey, currentValue) = values[i];
            currentKey *= 4;
            for (var j = 0; j < 4; j++)
            {
                var realKey = currentKey + j;
                var realValue = GetValueFromByte(currentValue[0], realKey);
                if (realValue == EmptyValue)
                {
                    continue;
                }

                result++;
            }
        }

        return result;
    }

    private async ValueTask WriteInternalAsync(int key, bool? value)
    {
        var byteArr = await _innerRepository.ReadAsync(key / 4, false);

        var buffer = (byte)0x0;

        if (!_innerRepository.IsValueEmpty(byteArr))
        {
            buffer = byteArr[0];
        }

        SetValueInByte(value, buffer, key);

        await _innerRepository.WriteAsync(key / 4, new[] { buffer }, false);
    }

    private void ClearInternal()
    {
        _innerRepository.Clear(false);
    }

    private bool? GetValueFromByte(byte buffer, int key)
    {
        var offset = key % 4 * 2;
        buffer >>= offset;
        buffer &= LocalMaskInternal;

        return buffer switch
        {
            NullValueInternal => null,
            FalseValueInternal => false,
            TrueValueInternal => true,
            _ => throw new InternalPersistifyException()
        };
    }

    private byte SetValueInByte(bool? value, byte buffer, int key)
    {
        var offset = key % 4 * 2;
        var bufferToWrite = value switch
        {
            null => NullValueInternal,
            false => FalseValueInternal,
            true => TrueValueInternal
        };

        bufferToWrite <<= offset;
        var mask = LocalMaskInternal << offset;
        mask = ~mask;
        buffer &= (byte)mask;
        buffer |= bufferToWrite;

        return buffer;
    }
}
