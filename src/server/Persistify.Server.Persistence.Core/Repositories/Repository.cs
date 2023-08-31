using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.Persistence.Core.Transactions;

namespace Persistify.Server.Persistence.Core.Repositories;

public abstract class Repository : IRepository
{
    private readonly ReadWriteAsyncLock _readWriteAsyncLock;
    protected Queue<Func<ValueTask>> PendingActions { get; }
    private static ulong TransactionId => Transaction.CurrentTransactionId;

    protected Repository()
    {
        _readWriteAsyncLock = new ReadWriteAsyncLock();
        PendingActions = new Queue<Func<ValueTask>>();
    }

    public async ValueTask<bool> BeginReadAsync(TimeSpan timeOut, CancellationToken cancellationToken)
    {
        return await _readWriteAsyncLock.EnterReadLockAsync(TransactionId, timeOut, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<bool> BeginWriteAsync(TimeSpan timeOut, CancellationToken cancellationToken)
    {
        return await _readWriteAsyncLock.EnterWriteLockAsync(TransactionId, timeOut, cancellationToken);
    }

    public async ValueTask EndReadAsync()
    {
        await _readWriteAsyncLock.ExitReadLockAsync(TransactionId).ConfigureAwait(false);
    }

    public async ValueTask EndWriteAsync()
    {
        await _readWriteAsyncLock.ExitWriteLockAsync(TransactionId).ConfigureAwait(false);
    }

    public async ValueTask ExecutePendingActionsAsync()
    {
        foreach (var pendingAction in PendingActions)
        {
            await pendingAction();
        }
    }

    public void ClearPendingActions()
    {
        PendingActions.Clear();
    }

    protected bool CanRead()
    {
        return _readWriteAsyncLock.CanRead(TransactionId);
    }

    protected bool CanWrite()
    {
        return _readWriteAsyncLock.CanWrite(TransactionId);
    }
}
