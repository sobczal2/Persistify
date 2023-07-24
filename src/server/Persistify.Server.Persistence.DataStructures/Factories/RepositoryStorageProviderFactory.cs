using System.Collections.Concurrent;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.DataStructures.Providers;

namespace Persistify.Server.Persistence.DataStructures.Factories;

public class RepositoryStorageProviderFactory : IStorageProviderFactory
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly ConcurrentDictionary<string, object> _providers;

    public RepositoryStorageProviderFactory(
        IRepositoryFactory repositoryFactory
    )
    {
        _repositoryFactory = repositoryFactory;
        _providers = new ConcurrentDictionary<string, object>();
    }

    public IStorageProvider<T> Create<T>(string name) where T : notnull
    {
        return (IStorageProvider<T>)_providers.GetOrAdd(
            name, static (_, args) => new RepositoryStorageProvider<T>(
                args.repositoryFactory.Create<T>(args.name)
            ),
            (name: name, repositoryFactory: _repositoryFactory)
        );
    }
}
