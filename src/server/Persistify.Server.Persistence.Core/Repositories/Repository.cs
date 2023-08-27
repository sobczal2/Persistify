using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Transactions;
using Persistify.Server.Persistence.Core.Transactions.Exceptions;

namespace Persistify.Server.Persistence.Core.Repositories;

public abstract class Repository : IRepository
{
    private readonly ReadWriteTransactionLock _readWriteTransactionLock;
    protected readonly Queue<Func<Task>> CommitQueue;

    public Repository()
    {
        _readWriteTransactionLock = new ReadWriteTransactionLock();
        CommitQueue = new Queue<Func<Task>>();
    }


    public async ValueTask BeginReadAsync(long transactionId)
    {
        await _readWriteTransactionLock.EnterReadLockAsync(transactionId).ConfigureAwait(false);
    }

    public async ValueTask BeginWriteAsync(long transactionId)
    {
        await _readWriteTransactionLock.EnterWriteLockAsync(transactionId).ConfigureAwait(false);
    }

    public async ValueTask EndReadAsync(long transactionId)
    {
        if (_readWriteTransactionLock.CanRead(transactionId))
        {
            await _readWriteTransactionLock.ExitReadLockAsync(transactionId).ConfigureAwait(false);
        }
        else
        {
            throw new TransactionStateCorruptedException();
        }
    }

    public async ValueTask EndWriteAsync(long transactionId)
    {
        if (_readWriteTransactionLock.CanWrite(transactionId))
        {
            CommitQueue.Clear();
            await _readWriteTransactionLock.ExitWriteLockAsync(transactionId).ConfigureAwait(false);
        }
        else
        {
            throw new TransactionStateCorruptedException();
        }
    }

    public async ValueTask FlushAsync()
    {
        while (CommitQueue.Count > 0)
        {
            var action = CommitQueue.Dequeue();
            await action().ConfigureAwait(false);
        }
    }

    protected bool CanRead()
    {
        return Transaction.CanReadGlobal() &&
               _readWriteTransactionLock.CanRead(Transaction.CurrentTransactionId ??
                                                 throw new TransactionStateCorruptedException());
    }

    protected bool CanWrite()
    {
        return Transaction.CanReadGlobal() &&
               _readWriteTransactionLock.CanWrite(Transaction.CurrentTransactionId ??
                                                  throw new TransactionStateCorruptedException());
    }
}
