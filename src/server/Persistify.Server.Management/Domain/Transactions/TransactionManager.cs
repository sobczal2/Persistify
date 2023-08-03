using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.Locking;
using Persistify.Server.Management.Abstractions.Domain;

namespace Persistify.Server.Management.Domain.Transactions;

public class TransactionManager : ITransactionManager
{
    private readonly ILogger<TransactionManager> _logger;
    private readonly ConcurrentDictionary<int, ReadWriteSemaphoreSlim> _templateIdToLockMap;
    private readonly ReadWriteSemaphoreSlim _globalLock;

    private static Transaction Transaction => TransactionState.RequiredCurrent;

    public TransactionManager(
        ILogger<TransactionManager> logger
    )
    {
        _logger = logger;
        _templateIdToLockMap = new ConcurrentDictionary<int, ReadWriteSemaphoreSlim>();
        _globalLock = new ReadWriteSemaphoreSlim();
    }

    public async ValueTask BeginAsync(IEnumerable<int> templateIds, bool write, bool global)
    {
        if (Transaction == null)
        {
            throw new InvalidOperationException("Transaction not available.");
        }

        Transaction.Write = write;
        Transaction.Global = global;
        Transaction.TemplateIds.AddRange(templateIds);

        if (global)
        {
            if (write)
                await _globalLock.EnterWriteLockAsync();
            else
                await _globalLock.EnterReadLockAsync();
        }

        foreach (var templateId in Transaction.TemplateIds)
        {
            var lockObject = _templateIdToLockMap.GetOrAdd(templateId, _ => new ReadWriteSemaphoreSlim());

            if (write)
            {
                await lockObject.EnterWriteLockAsync();
            }
            else
            {
                await lockObject.EnterReadLockAsync();
            }
        }
    }

    public async ValueTask CommitAsync()
    {
        if (Transaction == null)
        {
            throw new InvalidOperationException("Transaction not available.");
        }

        if (Transaction.Global)
        {
            if (Transaction.Write)
                _globalLock.ExitWriteLock();
            else
                await _globalLock.ExitReadLockAsync();
        }

        foreach (var templateId in Transaction.TemplateIds)
        {
            if (!_templateIdToLockMap.TryGetValue(templateId, out var lockObject))
            {
                throw new InvalidOperationException("Lock object not found.");
            }

            if (Transaction.Write)
            {
                lockObject.ExitWriteLock();
            }
            else
            {
                await lockObject.ExitReadLockAsync();
            }
        }
    }

    public async ValueTask RollbackAsync()
    {
        if (Transaction == null)
        {
            throw new InvalidOperationException("Transaction not available.");
        }

        try
        {
            while (Transaction.RollbackActions.Count > 0)
            {
                var action = Transaction.RollbackActions.Pop();
                await action();
            }
        }
        catch (Exception)
        {
            _logger.LogError("Rollback failed");
        }

        if (Transaction.Global)
        {
            if (Transaction.Write)
                _globalLock.ExitWriteLock();
            else
                await _globalLock.ExitReadLockAsync();
        }

        foreach (var templateId in Transaction.TemplateIds)
        {
            if (!_templateIdToLockMap.TryGetValue(templateId, out var lockObject))
            {
                throw new InvalidOperationException("Lock object not found.");
            }

            if (Transaction.Write)
            {
                lockObject.ExitWriteLock();
            }
            else
            {
                await lockObject.ExitReadLockAsync();
            }
        }
    }
}
