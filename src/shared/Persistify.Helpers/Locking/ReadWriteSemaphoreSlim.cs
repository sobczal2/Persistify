using System;
using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Helpers.Locking;

public class ReadWriteSemaphoreSlim : IDisposable
{
    private readonly SemaphoreSlim _readSemaphoreSlim;
    private readonly SemaphoreSlim _writeSemaphoreSlim;
    private int _readersCount;

    public ReadWriteSemaphoreSlim()
    {
        _readSemaphoreSlim = new SemaphoreSlim(1, 1);
        _writeSemaphoreSlim = new SemaphoreSlim(1, 1);
        _readersCount = 0;
    }

    public async ValueTask EnterReadLockAsync()
    {
        await _readSemaphoreSlim.WaitAsync().ConfigureAwait(false);

        if (Interlocked.Increment(ref _readersCount) == 1)
        {
            await _writeSemaphoreSlim.WaitAsync().ConfigureAwait(false);
        }

        _readSemaphoreSlim.Release();
    }

    public async ValueTask ExitReadLockAsync()
    {
        await _readSemaphoreSlim.WaitAsync().ConfigureAwait(false);

        if (Interlocked.Decrement(ref _readersCount) == 0)
        {
            _readSemaphoreSlim.Release();
        }

        _readSemaphoreSlim.Release();
    }

    public async ValueTask EnterWriteLockAsync()
    {
        await _writeSemaphoreSlim.WaitAsync().ConfigureAwait(false);
    }

    public void ExitWriteLock()
    {
        _writeSemaphoreSlim.Release();
    }

    public void Dispose()
    {
        _readSemaphoreSlim.Dispose();
        _writeSemaphoreSlim.Dispose();
    }
}
