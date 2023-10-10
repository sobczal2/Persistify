using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Abstractions;

namespace Persistify.Server.Persistence.Extensions;

public static class RefTypeStreamRepositoryExtensions
{
    public static async ValueTask<List<(int key, TValue value)>> ReadAllAsync<TValue>(
        this IRefTypeStreamRepository<TValue> repository,
        bool useLock
    )
        where TValue : class
    {
        return await repository.ReadRangeAsync(int.MaxValue, 0, useLock);
    }
}
