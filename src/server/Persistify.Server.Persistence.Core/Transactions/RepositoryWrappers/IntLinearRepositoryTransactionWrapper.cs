using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Transactions.TLogs;

namespace Persistify.Server.Persistence.Core.Transactions.RepositoryWrappers;

public class IntLinearRepositoryTransactionWrapper : IIntLinearRepository, IDisposable
{
    private readonly IIntLinearRepository _repository;
    private readonly Transaction _transaction;
    private readonly ISystemClock _systemClock;

    public IntLinearRepositoryTransactionWrapper(
        IIntLinearRepository repository,
        Transaction transaction,
        ISystemClock systemClock
    )
    {
        _repository = repository;
        _transaction = transaction;
        _systemClock = systemClock;
    }

    public async ValueTask<int?> ReadAsync(int key, bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.IntLinear,
                key,
                RepositoryAction.Read,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        return await _repository.ReadAsync(key, useLock);
    }

    public async ValueTask<IDictionary<int, int>> ReadAllAsync(bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.IntLinear,
                0,
                RepositoryAction.Read,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        return await _repository.ReadAllAsync(useLock);
    }

    public async ValueTask WriteAsync(int key, int value, bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.IntLinear,
                key,
                RepositoryAction.Write,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        var oldValue = await _repository.ReadAsync(key, useLock);
        if (oldValue.HasValue)
        {
            _transaction.RollbackActions.Push(
                async () => await _repository.WriteAsync(key, oldValue.Value, useLock)
            );
        }
        else
        {
            _transaction.RollbackActions.Push(
                async () => await _repository.DeleteAsync(key, useLock)
            );
        }

        await _repository.WriteAsync(key, value, useLock);
    }

    public async ValueTask DeleteAsync(int key, bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.IntLinear,
                key,
                RepositoryAction.Delete,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        var oldValue = await _repository.ReadAsync(key, useLock);
        if (oldValue.HasValue)
        {
            _transaction.RollbackActions.Push(
                async () => await _repository.WriteAsync(key, oldValue.Value, useLock)
            );
        }

        await _repository.DeleteAsync(key, useLock);
    }

    public void Clear(bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.IntLinear,
                0,
                RepositoryAction.Clear,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        _transaction.RollbackActions.Push(
            () => throw new NotSupportedException("Clear is not supported for rollback")
        );

        _repository.Clear(useLock);
    }

    public void Dispose()
    {
        (_repository as IDisposable)?.Dispose();
    }
}
