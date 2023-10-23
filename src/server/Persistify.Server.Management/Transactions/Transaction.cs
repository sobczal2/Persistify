using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Transactions.Exceptions;

namespace Persistify.Server.Management.Transactions;

// TODO: Handle timeout and cancellation token
public sealed class Transaction : ITransaction
{
    private readonly ILogger<Transaction> _logger;
    private readonly ITransactionDescriptor _transactionDescriptor;
    private readonly ITransactionState _transactionState;

    public Transaction(
        ITransactionDescriptor transactionDescriptor,
        ITransactionState transactionState,
        ILogger<Transaction> logger
    )
    {
        _transactionDescriptor =
            transactionDescriptor ?? throw new ArgumentNullException(nameof(transactionDescriptor));
        _transactionState = transactionState ?? throw new ArgumentNullException(nameof(transactionState));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        Id = Guid.NewGuid();
        Phase = TransactionPhase.Ready;
    }

    public Guid Id { get; }

    public TransactionPhase Phase { get; private set; }

    public async ValueTask BeginAsync(TimeSpan timeOut,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Begin transaction {TransactionId}", Id);

        if (Phase != TransactionPhase.Ready)
        {
            throw new InvalidOperationException("Transaction already started");
        }

        if (_transactionState.GetCurrentTransaction() != this)
        {
            throw new TransactionStateCorruptedException();
        }

        if (_transactionDescriptor.ExclusiveGlobal)
        {
            await _transactionState.EnterWriteGlobalLockAsync(Id, timeOut, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await _transactionState.EnterReadGlobalLockAsync(Id, timeOut, cancellationToken).ConfigureAwait(false);
        }

        foreach (var readManager in _transactionDescriptor.ReadManagers)
        {
            await readManager.BeginReadAsync(timeOut, cancellationToken).ConfigureAwait(false);
        }

        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            await writeManager.BeginWriteAsync(timeOut, cancellationToken).ConfigureAwait(false);
        }

        Phase = TransactionPhase.Started;

        _logger.LogDebug("Transaction {TransactionId} started", Id);
    }

    public async ValueTask PromoteManagerAsync(IManager manager, bool write, TimeSpan timeOut)
    {
        _logger.LogDebug("Promote manager {ManagerName} to {Write} in transaction {TransactionId}",
            manager.Name, write ? "write" : "read", Id);

        if (Phase != TransactionPhase.Started)
        {
            throw new InvalidOperationException("Transaction not started");
        }

        if (_transactionState.GetCurrentTransaction() != this)
        {
            throw new TransactionStateCorruptedException();
        }

        if (_transactionDescriptor.ReadManagers.Contains(manager))
        {
            throw new InvalidOperationException("Manager already promoted to read");
        }

        if (_transactionDescriptor.WriteManagers.Contains(manager))
        {
            throw new InvalidOperationException("Manager already promoted to write");
        }

        if (write)
        {
            await manager.BeginWriteAsync(timeOut, CancellationToken.None).ConfigureAwait(false);
            _transactionDescriptor.AddWriteManager(manager);
        }
        else
        {
            await manager.BeginReadAsync(timeOut, CancellationToken.None).ConfigureAwait(false);
            _transactionDescriptor.AddReadManager(manager);
        }

        _logger.LogDebug("Manager {ManagerName} promoted to {Write} in transaction {TransactionId}",
            manager.Name, write ? "write" : "read", Id);
    }

    public async ValueTask CommitAsync()
    {
        _logger.LogDebug("Commit transaction {TransactionId}", Id);

        if (Phase != TransactionPhase.Started)
        {
            throw new InvalidOperationException("Transaction not started");
        }

        if (_transactionState.GetCurrentTransaction() != this)
        {
            throw new TransactionStateCorruptedException();
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < _transactionDescriptor.WriteManagers.Count; i++)
        {
            var writeManager = _transactionDescriptor.WriteManagers[i];
            try
            {
                await writeManager.ExecutePendingActionsAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    "Error occurred while executing pending actions for manager {ManagerName} in transaction {TransactionId}",
                    writeManager.Name, Id);
                throw;
            }
        }

        foreach (var readManager in _transactionDescriptor.ReadManagers)
        {
            await readManager.EndReadAsync().ConfigureAwait(false);
        }

        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            await writeManager.EndWriteAsync().ConfigureAwait(false);
        }

        if (_transactionDescriptor.ExclusiveGlobal)
        {
            await _transactionState.ExitWriteGlobalLockAsync(Id).ConfigureAwait(false);
        }
        else
        {
            await _transactionState.ExitReadGlobalLockAsync(Id).ConfigureAwait(false);
        }

        Phase = TransactionPhase.Committed;

        _logger.LogDebug("Transaction {TransactionId} committed", Id);
    }

    public async ValueTask RollbackAsync()
    {
        _logger.LogDebug("Rollback transaction {TransactionId}", Id);

        if (Phase != TransactionPhase.Started)
        {
            throw new InvalidOperationException("Transaction not started");
        }

        if (_transactionState.GetCurrentTransaction() != this)
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
            await _transactionState.ExitWriteGlobalLockAsync(Id).ConfigureAwait(false);
        }
        else
        {
            await _transactionState.ExitReadGlobalLockAsync(Id).ConfigureAwait(false);
        }

        Phase = TransactionPhase.RolledBack;

        _logger.LogDebug("Transaction {TransactionId} rolled back", Id);
    }
}
