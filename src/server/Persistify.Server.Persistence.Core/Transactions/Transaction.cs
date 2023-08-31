using System;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Concurrency;
using Persistify.Server.Persistence.Core.Transactions.Exceptions;

namespace Persistify.Server.Persistence.Core.Transactions;

public sealed class Transaction
{
    private static readonly AsyncLocal<Transaction?> CurrentTransaction;
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
        if (CurrentTransaction.Value != null)
        {
            throw new TransactionStateCorruptedException();
        }

        CurrentTransaction.Value = this;

        if (_transactionDescriptor.ExclusiveGlobal)
        {
            await GlobalLock.EnterWriteLockAsync(_id, timeOut, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await GlobalLock.EnterReadLockAsync(_id, timeOut, cancellationToken).ConfigureAwait(false);
        }

        foreach (var readRepository in _transactionDescriptor.ReadRepositories)
        {
            await readRepository.BeginReadAsync(timeOut, cancellationToken).ConfigureAwait(false);
        }

        foreach (var writeRepository in _transactionDescriptor.WriteRepositories)
        {
            await writeRepository.BeginWriteAsync(timeOut, cancellationToken).ConfigureAwait(false);
        }
    }

    public async ValueTask CommitAsync()
    {
        if (CurrentTransaction.Value != this)
        {
            throw new TransactionStateCorruptedException();
        }

        foreach (var readRepository in _transactionDescriptor.ReadRepositories)
        {
            await readRepository.EndReadAsync().ConfigureAwait(false);
        }

        foreach (var writeRepository in _transactionDescriptor.WriteRepositories)
        {
            await writeRepository.ExecutePendingActionsAsync().ConfigureAwait(false);
            await writeRepository.EndWriteAsync().ConfigureAwait(false);
        }

        await GlobalLock.ExitReadLockAsync(_id).ConfigureAwait(false);


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

        foreach (var readRepository in _transactionDescriptor.ReadRepositories)
        {
            await readRepository.EndReadAsync().ConfigureAwait(false);
        }

        foreach (var writeRepository in _transactionDescriptor.WriteRepositories)
        {
            writeRepository.ClearPendingActions();
            await writeRepository.EndWriteAsync().ConfigureAwait(false);
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
