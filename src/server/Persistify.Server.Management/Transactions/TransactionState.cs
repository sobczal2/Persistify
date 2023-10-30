using System;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.Management.Transactions.Exceptions;

namespace Persistify.Server.Management.Transactions;

public class TransactionState : ITransactionState
{
    private readonly ReadWriteAsyncLock _globalLock;

    public TransactionState()
    {
        CurrentTransaction = new AsyncLocal<ITransaction?>();
        _globalLock = new ReadWriteAsyncLock();
    }

    public AsyncLocal<ITransaction?> CurrentTransaction { get; }

    public ITransaction GetCurrentTransaction()
    {
        return CurrentTransaction.Value ?? throw new TransactionStateCorruptedException();
    }

    public async ValueTask<bool> EnterReadGlobalLockAsync(
        Guid transactionId,
        TimeSpan timeOut,
        CancellationToken cancellationToken
    )
    {
        return await _globalLock
            .EnterReadLockAsync(transactionId, timeOut, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<bool> EnterWriteGlobalLockAsync(
        Guid transactionId,
        TimeSpan timeOut,
        CancellationToken cancellationToken
    )
    {
        return await _globalLock
            .EnterWriteLockAsync(transactionId, timeOut, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask ExitReadGlobalLockAsync(Guid transactionId)
    {
        await _globalLock.ExitReadLockAsync(transactionId).ConfigureAwait(false);
    }

    public async ValueTask ExitWriteGlobalLockAsync(Guid transactionId)
    {
        await _globalLock.ExitWriteLockAsync(transactionId).ConfigureAwait(false);
    }
}
