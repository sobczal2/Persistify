using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Transactions.TLogs;

namespace Persistify.Server.Persistence.Core.Transactions.RepositoryWrappers;

public class RepositoryTransactionWrapper<T> : IRepository<T>, IDisposable
    where T : class
{
    private readonly IRepository<T> _repository;
    private readonly Transaction _transaction;
    private readonly ISystemClock _systemClock;

    public RepositoryTransactionWrapper(
        IRepository<T> repository,
        Transaction transaction,
        ISystemClock systemClock
    )
    {
        _repository = repository;
        _transaction = transaction;
        _systemClock = systemClock;
    }

    public async ValueTask<T?> ReadAsync(int id, bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.Object,
                id,
                RepositoryAction.Read,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        return await _repository.ReadAsync(id, useLock);
    }

    public async ValueTask<IDictionary<int, T>> ReadAllAsync(bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.Object,
                0,
                RepositoryAction.Read,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        return await _repository.ReadAllAsync(useLock);
    }

    public async ValueTask WriteAsync(int id, T value, bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.Object,
                id,
                RepositoryAction.Write,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        var oldValue = await _repository.ReadAsync(id, useLock);

        if (oldValue is not null)
        {
            _transaction.RollbackActions.Push(
                async () => await _repository.WriteAsync(id, oldValue, useLock));
        }
        else
        {
            _transaction.RollbackActions.Push(
                async () => await _repository.DeleteAsync(id, useLock));
        }

        await _repository.WriteAsync(id, value, useLock);
    }

    public async ValueTask<bool> DeleteAsync(int id, bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.Object,
                id,
                RepositoryAction.Delete,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        var oldValue = await _repository.ReadAsync(id, useLock);

        if (oldValue is not null)
        {
            _transaction.RollbackActions.Push(
                async () => await _repository.WriteAsync(id, oldValue, useLock));
        }

        return await _repository.DeleteAsync(id, useLock);
    }

    public void Clear(bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.Object,
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
