using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.Persistence.Abstractions;

namespace Persistify.Server.Persistence.Primitives;

public abstract class ByteArrayBasedStreamRepository<TValue>
    : IValueTypeStreamRepository<TValue>,
        IDisposable
{
    private readonly ByteArrayStreamRepository _innerRepository;
    private readonly SemaphoreSlim _semaphore;

    public ByteArrayBasedStreamRepository(
        Stream stream,
        int size
    )
    {
        _innerRepository = new ByteArrayStreamRepository(stream, size);
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public void Dispose()
    {
        _innerRepository.Dispose();
        _semaphore.Dispose();
    }

    public TValue EmptyValue => BytesToValue(_innerRepository.EmptyValue);

    public async ValueTask<TValue> ReadAsync(
        int key,
        bool useLock
    )
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock
            ? await _semaphore.WrapAsync(async () => await ReadAsyncImpl(key))
            : await ReadAsyncImpl(key);
    }

    public async IAsyncEnumerable<(int key, TValue value)> ReadRangeAsync(
        int take,
        int skip,
        bool useLock
    )
    {
        if (take <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(take));
        }

        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip));
        }

        if (useLock)
        {
            await _semaphore.WaitAsync();
            try
            {
                await foreach (var result in ReadRangeAsyncImpl(take, skip))
                {
                    yield return result;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            yield break;
        }

        await foreach (var result in ReadRangeAsyncImpl(take, skip))
        {
            yield return result;
        }
    }

    public async ValueTask<int> CountAsync(
        bool useLock
    )
    {
        return useLock ? await _semaphore.WrapAsync(CountAsyncImpl) : await CountAsyncImpl();
    }

    public async ValueTask WriteAsync(
        int key,
        TValue value,
        bool useLock
    )
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        if (IsValueEmpty(value))
        {
            throw new ArgumentException(nameof(value));
        }

        await (
            useLock
                ? _semaphore.WrapAsync(() => WriteAsyncImpl(key, value))
                : WriteAsyncImpl(key, value)
        );
    }

    public async ValueTask<bool> DeleteAsync(
        int key,
        bool useLock
    )
    {
        if (key < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        return useLock
            ? await _semaphore.WrapAsync(() => DeleteAsyncImpl(key))
            : await DeleteAsyncImpl(key);
    }

    public void Clear(
        bool useLock
    )
    {
        if (useLock)
        {
            _semaphore.Wrap(ClearImpl);
        }
        else
        {
            ClearImpl();
        }
    }

    public bool IsValueEmpty(
        TValue value
    )
    {
        return _innerRepository.IsValueEmpty(ValueToBytes(value));
    }

    protected abstract TValue BytesToValue(
        byte[] bytes
    );

    protected abstract byte[] ValueToBytes(
        TValue value
    );

    private async ValueTask<TValue> ReadAsyncImpl(
        int key
    )
    {
        var bytes = await _innerRepository.ReadAsync(key, false);
        return BytesToValue(bytes);
    }

    private async IAsyncEnumerable<(int key, TValue value)> ReadRangeAsyncImpl(
        int take,
        int skip
    )
    {
        await foreach (var (key, bytes) in _innerRepository.ReadRangeAsync(take, skip, false))
        {
            yield return (key, BytesToValue(bytes));
        }
    }

    private async ValueTask WriteAsyncImpl(
        int key,
        TValue value
    )
    {
        var bytes = ValueToBytes(value);
        await _innerRepository.WriteAsync(key, bytes, false);
    }

    private async ValueTask<int> CountAsyncImpl()
    {
        return await _innerRepository.CountAsync(false);
    }

    private async ValueTask<bool> DeleteAsyncImpl(
        int key
    )
    {
        return await _innerRepository.DeleteAsync(key, false);
    }

    private void ClearImpl()
    {
        _innerRepository.Clear(false);
    }
}
