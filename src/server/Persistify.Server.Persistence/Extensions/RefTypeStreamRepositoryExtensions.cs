using System.Collections.Generic;
using Persistify.Server.Persistence.Abstractions;

namespace Persistify.Server.Persistence.Extensions;

public static class RefTypeStreamRepositoryExtensions
{
    public static IAsyncEnumerable<(int key, TValue value)> ReadAllAsync<TValue>(
        this IRefTypeStreamRepository<TValue> repository,
        bool useLock
    )
        where TValue : class
    {
        return repository.ReadRangeAsync(int.MaxValue, 0, useLock);
    }
}
