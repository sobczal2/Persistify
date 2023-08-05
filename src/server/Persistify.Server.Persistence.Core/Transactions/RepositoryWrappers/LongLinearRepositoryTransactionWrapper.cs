using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Transactions.TLogs;

namespace Persistify.Server.Persistence.Core.Transactions.RepositoryWrappers;

public class LongLinearRepositoryTransactionWrapper : ILongLinearRepository, IDisposable
{
    private readonly ILongLinearRepository _repository;
    private readonly Transaction _transaction;
    private readonly ISystemClock _systemClock;

    public LongLinearRepositoryTransactionWrapper(
        ILongLinearRepository repository,
        Transaction transaction,
        ISystemClock systemClock
        )
    {
        _repository = repository;
        _transaction = transaction;
        _systemClock = systemClock;
    }
    public async ValueTask<long?> ReadAsync(int key, bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.LongLinear,
                key,
                RepositoryAction.Read,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        return await _repository.ReadAsync(key, useLock);
    }

    public async ValueTask<IDictionary<int, long>> ReadAllAsync(bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.LongLinear,
                0,
                RepositoryAction.Read,
                _systemClock.UtcNow.UtcDateTime
            )
        );

        return await _repository.ReadAllAsync(useLock);
    }

    public async ValueTask WriteAsync(int key, long value, bool useLock = true)
    {
        _transaction.TransactionLogs.Add(
            new RepositoryLog(
                RepositoryType.LongLinear,
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
                RepositoryType.LongLinear,
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
                RepositoryType.LongLinear,
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
