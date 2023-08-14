using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Transactions.Exceptions;

namespace Persistify.Server.Persistence.Core.Transactions;

public class Transaction
{
    private static readonly AsyncLocal<Transaction?> CurrentTransaction;
    private static long _lastTransactionId;
    private static readonly ReadWriteTransactionLock GlobalLock;

    public static long? CurrentTransactionId => CurrentTransaction.Value?.Id;

    static Transaction()
    {
        CurrentTransaction = new AsyncLocal<Transaction?>();
        _lastTransactionId = 0;
        var tmp = _lastTransactionId;
        GlobalLock = new ReadWriteTransactionLock();
    }

    public static bool CanReadGlobal()
    {
        if (CurrentTransaction.Value is null)
        {
            return false;
        }

        return GlobalLock.CanRead(CurrentTransaction.Value.Id);
    }

    public static bool CanWriteGlobal()
    {
        if (CurrentTransaction.Value is null)
        {
            return false;
        }

        return GlobalLock.CanWrite(CurrentTransaction.Value.Id);
    }

    public long Id { get; private set; }
    private readonly TransactionDescriptor _transactionDescriptor;
    private Queue<Func<Task>> _queuedCommitActions;

    public Transaction(
        TransactionDescriptor transactionDescriptor
    )
    {
        _transactionDescriptor = transactionDescriptor;
        Id = Interlocked.Increment(ref _lastTransactionId);
        _queuedCommitActions = new Queue<Func<Task>>();
    }

    public async ValueTask BeginAsync()
    {
        if (CurrentTransaction.Value is not null)
        {
            throw new TransactionStateCorruptedException();
        }

        CurrentTransaction.Value = this;

        await GlobalLock.EnterReadLockAsync(Id).ConfigureAwait(false);

        if (_transactionDescriptor.WriteGlobal)
        {
            await GlobalLock.EnterWriteLockAsync(Id).ConfigureAwait(false);
        }

        foreach (var readRepository in _transactionDescriptor.ReadRepositories)
        {
            await readRepository.BeginReadAsync(Id).ConfigureAwait(false);
        }

        foreach (var writeRepository in _transactionDescriptor.WriteRepositories)
        {
            await writeRepository.BeginWriteAsync(Id).ConfigureAwait(false);
        }
    }

    public async ValueTask CommitAsync()
    {
        if (CurrentTransaction.Value != this)
        {
            throw new TransactionStateCorruptedException();
        }

        foreach (var commitAction in _queuedCommitActions)
        {
            await commitAction().ConfigureAwait(false);
        }

        foreach (var readRepository in _transactionDescriptor.ReadRepositories)
        {
            await readRepository.EndReadAsync(Id).ConfigureAwait(false);
        }

        foreach (var writeRepository in _transactionDescriptor.WriteRepositories)
        {
            await writeRepository.FlushAsync().ConfigureAwait(false);
            await writeRepository.EndWriteAsync(Id).ConfigureAwait(false);
        }

        await GlobalLock.ExitReadLockAsync(Id).ConfigureAwait(false);


        if (_transactionDescriptor.WriteGlobal)
        {
            await GlobalLock.ExitWriteLockAsync(Id).ConfigureAwait(false);
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
            await readRepository.EndReadAsync(Id).ConfigureAwait(false);
        }

        foreach (var writeRepository in _transactionDescriptor.WriteRepositories)
        {
            await writeRepository.EndWriteAsync(Id).ConfigureAwait(false);
        }

        await GlobalLock.ExitReadLockAsync(Id).ConfigureAwait(false);

        if (_transactionDescriptor.WriteGlobal)
        {
            await GlobalLock.ExitWriteLockAsync(Id).ConfigureAwait(false);
        }

        CurrentTransaction.Value = null;
    }
}
