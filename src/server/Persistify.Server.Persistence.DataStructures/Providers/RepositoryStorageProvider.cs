using System;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Persistence.DataStructures.Providers;

public class RepositoryStorageProvider<T> : IStorageProvider<T>
    where T : notnull
{
    private readonly IRepository<T> _repository;

    public RepositoryStorageProvider(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async ValueTask WriteAsync(long id, T value)
    {
        await _repository.WriteAsync(id, value);
    }

    public async ValueTask<T?> ReadAsync(long id)
    {
        return await _repository.ReadAsync(id);
    }

    public async ValueTask RemoveAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }
}
