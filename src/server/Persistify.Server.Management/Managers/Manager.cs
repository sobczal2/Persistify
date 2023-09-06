using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.Errors;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Management.Transactions.Exceptions;

namespace Persistify.Server.Management.Managers;

public abstract class Manager : IManager
{
    protected readonly ITransactionState TransactionState;
    private readonly ReadWriteAsyncLock _readWriteAsyncLock;
    protected Queue<Func<ValueTask>> PendingActions { get; }
    private bool _isInitialized;

    private Guid TransactionId => TransactionState.GetCurrentTransaction().Id;

    // TODO: move to config
    protected TimeSpan TransactionTimeout => TimeSpan.FromSeconds(30);

    protected Manager(
        ITransactionState transactionState
        )
    {
        TransactionState = transactionState;
        _readWriteAsyncLock = new ReadWriteAsyncLock();
        PendingActions = new Queue<Func<ValueTask>>();
        _isInitialized = false;
    }

    public abstract string Name { get; }

    public virtual void Initialize()
    {
        _isInitialized = true;
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
        while (PendingActions.Count > 0)
        {
            var action = PendingActions.Dequeue();
            await action().ConfigureAwait(false);
        }
    }

    public void ClearPendingActions()
    {
        PendingActions.Clear();
    }

    protected void ThrowIfCannotRead()
    {
        if (!_readWriteAsyncLock.CanRead(TransactionId))
        {
            throw new NotAllowedForTransactionException();
        }
    }

    protected void ThrowIfCannotWrite()
    {
        if (!_readWriteAsyncLock.CanWrite(TransactionId))
        {
            throw new NotAllowedForTransactionException();
        }
    }

    protected void ThrowIfNotInitialized()
    {
        if (!_isInitialized)
        {
            throw new PersistifyInternalException();
        }
    }
}
