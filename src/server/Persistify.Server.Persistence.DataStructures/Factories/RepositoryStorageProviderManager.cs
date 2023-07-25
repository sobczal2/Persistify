using System;
using System.Collections.Concurrent;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.DataStructures.Providers;

namespace Persistify.Server.Persistence.DataStructures.Factories;

public class RepositoryStorageProviderManager : IStorageProviderManager
{
    private readonly ConcurrentDictionary<string, object> _providers;
    private readonly IRepositoryManager _repositoryManager;
    private readonly object _lock;

    public RepositoryStorageProviderManager(
        IRepositoryManager repositoryManager
    )
    {
        _repositoryManager = repositoryManager;
        _providers = new ConcurrentDictionary<string, object>();
        _lock = new object();
    }
    
    public void Create<T>(string name) where T : notnull
    {
        lock(_lock)
        {
            if (_providers.ContainsKey(name))
            {
                throw new InvalidOperationException($"Storage provider with name {name} already exists.");
            }

            _repositoryManager.Create<T>(name);
            _providers.TryAdd(name, new RepositoryStorageProvider<T>(_repositoryManager.Get<T>(name)));
        }
    }

    public IStorageProvider<T> Get<T>(string name) where T : notnull
    {
        if (!_providers.TryGetValue(name, out var provider))
        {
            throw new InvalidOperationException($"Storage provider with name {name} does not exist.");
        }

        return (IStorageProvider<T>)provider;
    }

    public void Delete<T>(string name) where T : notnull
    {
        lock(_lock)
        {
            if (!_providers.TryRemove(name, out _))
            {
                throw new InvalidOperationException($"Storage provider with name {name} does not exist.");
            }

            _repositoryManager.Delete<T>(name);
        }
    }
}
