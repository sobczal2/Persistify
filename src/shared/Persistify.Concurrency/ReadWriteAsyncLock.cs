using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Concurrency;

/// <summary>
///     This class is used to lock a resource for reading or writing.
///     Unsafe to use in a recursive manner.
///     Nested read locks are allowed.
///     Nested write locks are not allowed.
/// </summary>
public sealed class ReadWriteAsyncLock : IDisposable
{
    private readonly SemaphoreSlim _accessSemaphoreSlim;
    private readonly HashSet<Guid> _readers;
    private readonly SemaphoreSlim _readSemaphoreSlim;
    private readonly SemaphoreSlim _writeSemaphoreSlim;
    private volatile uint _pendingWriters;
    private Guid? _writer;

    public ReadWriteAsyncLock()
    {
        _readSemaphoreSlim = new SemaphoreSlim(1, 1);
        _writeSemaphoreSlim = new SemaphoreSlim(1, 1);
        _accessSemaphoreSlim = new SemaphoreSlim(1, 1);
        _readers = new HashSet<Guid>();
        _writer = null;
        _pendingWriters = 0;
    }

    public void Dispose()
    {
        _readSemaphoreSlim.Dispose();
        _writeSemaphoreSlim.Dispose();
        _accessSemaphoreSlim.Dispose();
    }

    public async ValueTask<bool> EnterReadLockAsync(Guid id, TimeSpan timeout, CancellationToken cancellationToken)
    {
        if (!await _accessSemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        while (_pendingWriters > 0)
        {
            _accessSemaphoreSlim.Release();
            if (!await _accessSemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(false))
            {
                return false;
            }
        }

        if (!await _readSemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(false))
        {
            _accessSemaphoreSlim.Release();
            return false;
        }

        if (_readers.Contains(id))
        {
            _readSemaphoreSlim.Release();
            _accessSemaphoreSlim.Release();
            throw new InvalidOperationException("Cannot enter read lock when already in read lock");
        }

        if (_readers.Count == 0)
        {
            await _writeSemaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
        }

        _readers.Add(id);

        _readSemaphoreSlim.Release();
        _accessSemaphoreSlim.Release();

        return true;
    }

    public async ValueTask ExitReadLockAsync(Guid id)
    {
        await _readSemaphoreSlim.WaitAsync().ConfigureAwait(false);

        if (!_readers.Contains(id))
        {
            _readSemaphoreSlim.Release();
            throw new InvalidOperationException("Cannot exit read lock when not in read lock");
        }

        if (_writer == id)
        {
            _readSemaphoreSlim.Release();
            throw new InvalidOperationException("Cannot exit read lock when in write lock");
        }

        _readers.Remove(id);

        if (_readers.Count == 0)
        {
            _writeSemaphoreSlim.Release();
        }

        _readSemaphoreSlim.Release();
    }

    public async ValueTask<bool> EnterWriteLockAsync(Guid id, TimeSpan timeout, CancellationToken cancellationToken)
    {
        if (!await _accessSemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        Interlocked.Increment(ref _pendingWriters);

        if (!await _writeSemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(false))
        {
            Interlocked.Decrement(ref _pendingWriters);
            _accessSemaphoreSlim.Release();
            return false;
        }

        _writer = id;

        return true;
    }

    public ValueTask ExitWriteLockAsync(Guid id)
    {
        if (_writer != id)
        {
            throw new InvalidOperationException("Cannot exit write lock when not in write lock");
        }

        _writer = null;
        Interlocked.Decrement(ref _pendingWriters);

        _writeSemaphoreSlim.Release();
        _accessSemaphoreSlim.Release();

        return ValueTask.CompletedTask;
    }

    public bool CanRead(Guid id)
    {
        return _readers.Contains(id) || _writer == id;
    }

    public bool CanWrite(Guid id)
    {
        return _writer == id;
    }
}
