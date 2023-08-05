using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.Locking;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Persistence.Core.Transactions;

public class TransactionManager : ITransactionManager
{
    private readonly ILogger<TransactionManager> _logger;
    private readonly ISystemClock _systemClock;
    private readonly ConcurrentDictionary<int, ReadWriteSemaphoreSlim> _templateIdToLockMap;
    private readonly ReadWriteSemaphoreSlim _globalLock;

    private readonly IRepository<Transaction> _transactionRepository;
    private readonly ILongLinearRepository _transactionIdRepository;
    private readonly SemaphoreSlim _transactionIdLock;

    private const string TransactionRepositoryName = "Transaction";
    private const string TransactionIdRepositoryName = "TransactionId";

    public Transaction Transaction => TransactionState.RequiredCurrent;

    public TransactionManager(
        ILogger<TransactionManager> logger,
        IRepositoryManager repositoryManager,
        ILongLinearRepositoryManager longLinearRepositoryManager,
        ISystemClock systemClock
    )
    {
        _logger = logger;
        _systemClock = systemClock;
        _templateIdToLockMap = new ConcurrentDictionary<int, ReadWriteSemaphoreSlim>();
        _globalLock = new ReadWriteSemaphoreSlim();

        repositoryManager.Create<Transaction>(TransactionRepositoryName);
        _transactionRepository = repositoryManager.Get<Transaction>(TransactionRepositoryName);

        longLinearRepositoryManager.Create(TransactionIdRepositoryName);
        _transactionIdRepository = longLinearRepositoryManager.Get(TransactionIdRepositoryName);

        _transactionIdLock = new SemaphoreSlim(1, 1);
    }

    private async ValueTask<long> GetNextTransactionIdAsync()
    {
        await _transactionIdLock.WaitAsync();
        try
        {
            var currentId = await _transactionIdRepository.ReadAsync(0) ?? 0;
            var nextId = currentId + 1;
            await _transactionIdRepository.WriteAsync(0, nextId);
            return nextId;
        }
        finally
        {
            _transactionIdLock.Release();
        }
    }

    public async ValueTask BeginAsync(IEnumerable<int> templateIds, bool write, bool global)
    {
        if (Transaction == null)
        {
            throw new InvalidOperationException("Transaction not available.");
        }

        Transaction.Id = await GetNextTransactionIdAsync();
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

        Transaction.StartTimestamp = _systemClock.UtcNow.UtcDateTime;
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

        Transaction.EndTimestamp = _systemClock.UtcNow.UtcDateTime;
        Transaction.Committed = true;
        await _transactionRepository.WriteAsync((int)(Transaction.Id % int.MaxValue), Transaction);

        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        _logger.LogInformation(Transaction.ToString());
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


        Transaction.EndTimestamp = _systemClock.UtcNow.UtcDateTime;
        Transaction.RolledBack = true;
        await _transactionRepository.WriteAsync((int)(Transaction.Id % int.MaxValue), Transaction);

        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        _logger.LogInformation(Transaction.ToString());
    }
}
