using System.Collections.Concurrent;
using System.Collections.Generic;
using Persistify.Server.Persistence.DataStructures.Abstractions;
using Persistify.Server.Persistence.DataStructures.Providers;
using Persistify.Server.Persistence.DataStructures.Trees;

namespace Persistify.Server.Persistence.DataStructures.Factories;

public class AvlAsyncTreeFactory : IAsyncTreeFactory
{
    private readonly IStorageProviderFactory _storageProviderFactory;
    private readonly ConcurrentDictionary<string, object> _trees;

    public AvlAsyncTreeFactory(
        IStorageProviderFactory storageProviderFactory
    )
    {
        _storageProviderFactory = storageProviderFactory;
        _trees = new ConcurrentDictionary<string, object>();
    }

    public IAsyncTree<T> Create<T>(string name, IComparer<T> comparer)
        where T : notnull
    {
        return (IAsyncTree<T>)_trees.GetOrAdd(
            name, static (_, args) => new AvlAsyncTree<T>(
                args.storageProviderFactory.Create<AvlAsyncTree<T>.Node>(args.name),
                args.comparer
            ), (name: name, comparer: comparer, storageProviderFactory: _storageProviderFactory)
        );
    }
}
