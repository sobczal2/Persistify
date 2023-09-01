using System;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.Management.Transactions.Exceptions;

namespace Persistify.Server.Management.Transactions;

public sealed class Transaction
{
    public static readonly AsyncLocal<Transaction?> CurrentTransaction;
    private static readonly ReadWriteAsyncLock GlobalLock;
    private static ulong _lastTransactionId;

    public static ulong CurrentTransactionId => CurrentTransaction.Value?._id ?? throw new TransactionStateCorruptedException();

    static Transaction()
    {
        CurrentTransaction = new AsyncLocal<Transaction?>();
        _lastTransactionId = 0;
        GlobalLock = new ReadWriteAsyncLock();
    }

    private readonly ulong _id;
    private readonly TransactionDescriptor _transactionDescriptor;

    public Transaction(
        TransactionDescriptor transactionDescriptor
    )
    {
        _transactionDescriptor = transactionDescriptor;
        _id = Interlocked.Increment(ref _lastTransactionId);
    }

    // TODO: handle timeout and cancellation token
    public async ValueTask BeginAsync(TimeSpan timeOut, CancellationToken cancellationToken)
    {
        if (CurrentTransaction.Value != this)
        {
            throw new TransactionStateCorruptedException();
        }

        if (_transactionDescriptor.ExclusiveGlobal)
        {
            await GlobalLock.EnterWriteLockAsync(_id, timeOut, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await GlobalLock.EnterReadLockAsync(_id, timeOut, cancellationToken).ConfigureAwait(false);
        }

        foreach (var readManager in _transactionDescriptor.ReadManagers)
        {
            await readManager.BeginReadAsync(timeOut, cancellationToken).ConfigureAwait(false);
        }

        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            await writeManager.BeginWriteAsync(timeOut, cancellationToken).ConfigureAwait(false);
        }
    }

    public async ValueTask CommitAsync()
    {
        if (CurrentTransaction.Value != this)
        {
            throw new TransactionStateCorruptedException();
        }

        foreach (var readManager in _transactionDescriptor.ReadManagers)
        {
            await readManager.EndReadAsync().ConfigureAwait(false);
        }

        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            await writeManager.ExecutePendingActionsAsync().ConfigureAwait(false);
            await writeManager.EndWriteAsync().ConfigureAwait(false);
        }

        if (_transactionDescriptor.ExclusiveGlobal)
        {
            await GlobalLock.ExitWriteLockAsync(_id).ConfigureAwait(false);
        }
        else
        {
            await GlobalLock.ExitReadLockAsync(_id).ConfigureAwait(false);
        }

        CurrentTransaction.Value = null;
    }

    public async ValueTask RollbackAsync()
    {
        if (CurrentTransaction.Value != this)
        {
            throw new TransactionStateCorruptedException();
        }

        foreach (var readManager in _transactionDescriptor.ReadManagers)
        {
            await readManager.EndReadAsync().ConfigureAwait(false);
        }

        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            writeManager.ClearPendingActions();
            await writeManager.EndWriteAsync().ConfigureAwait(false);
        }

        if (_transactionDescriptor.ExclusiveGlobal)
        {
            await GlobalLock.ExitWriteLockAsync(_id).ConfigureAwait(false);
        }
        else
        {
            await GlobalLock.ExitReadLockAsync(_id).ConfigureAwait(false);
        }

        CurrentTransaction.Value = null;
    }
}
