using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Concurrency;
using Persistify.Server.Management.Transactions.Exceptions;

namespace Persistify.Server.Management.Transactions;

public sealed class Transaction
{
    public static readonly AsyncLocal<Transaction?> CurrentTransaction;
    private static readonly ReadWriteAsyncLock GlobalLock;
    private static ulong _lastTransactionId;

    public static ulong CurrentTransactionId =>
        CurrentTransaction.Value?._id ?? throw new TransactionStateCorruptedException();

    static Transaction()
    {
        CurrentTransaction = new AsyncLocal<Transaction?>();
        _lastTransactionId = 0;
        GlobalLock = new ReadWriteAsyncLock();
    }

    private readonly ulong _id;
    private readonly TransactionDescriptor _transactionDescriptor;
    private readonly ILogger<Transaction> _logger;

    public Transaction(
        TransactionDescriptor transactionDescriptor,
        ILogger<Transaction> logger
        )
    {
        _transactionDescriptor = transactionDescriptor;
        _logger = logger;
        _id = Interlocked.Increment(ref _lastTransactionId);
    }

    // TODO: handle timeout and cancellation token
    public async ValueTask BeginAsync(TimeSpan timeOut, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Begin transaction {TransactionId}", _id);

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

        _logger.LogDebug("Transaction {TransactionId} started", _id);
    }

    public async ValueTask CommitAsync()
    {
        _logger.LogDebug("Commit transaction {TransactionId}", _id);

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
            try
            {
                await writeManager.ExecutePendingActionsAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                _logger.LogError("Error while executing pending actions for transaction {TransactionId} on manager {ManagerName}", _id, writeManager.GetType().Name);
            }

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

        _logger.LogDebug("Transaction {TransactionId} committed", _id);
    }

    public async ValueTask RollbackAsync()
    {
        _logger.LogDebug("Rollback transaction {TransactionId}", _id);

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

        _logger.LogDebug("Transaction {TransactionId} rolled back", _id);
    }
}
