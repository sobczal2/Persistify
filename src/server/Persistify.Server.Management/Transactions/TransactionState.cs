using System;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.Management.Transactions.Exceptions;

namespace Persistify.Server.Management.Transactions;

public class TransactionState : ITransactionState
{
    public AsyncLocal<ITransaction?> CurrentTransaction { get; }
    private readonly ReadWriteAsyncLock _globalLock;

    public TransactionState()
    {
        CurrentTransaction = new AsyncLocal<ITransaction?>();
        _globalLock = new ReadWriteAsyncLock();
    }

    public ITransaction GetCurrentTransaction()
    {
        return CurrentTransaction.Value ?? throw new TransactionStateCorruptedException();
    }

    public async ValueTask EnterReadGlobalLockAsync(Guid transactionId, TimeSpan timeOut, CancellationToken cancellationToken)
    {
        await _globalLock.EnterReadLockAsync(transactionId, timeOut, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask EnterWriteGlobalLockAsync(Guid transactionId, TimeSpan timeOut, CancellationToken cancellationToken)
    {
        await _globalLock.EnterWriteLockAsync(transactionId, timeOut, cancellationToken).ConfigureAwait(false);
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
