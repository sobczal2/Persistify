using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Persistence.DataStructures.Abstractions;
using Persistify.Server.Persistence.DataStructures.Providers;

namespace Persistify.Server.Persistence.DataStructures.Trees;

public class AvlAsyncRangeTree<T> : AvlAsyncTree<T>, IAsyncRangeTree<T>
    where T : IComparable<double>
{
    public AvlAsyncRangeTree(IStorageProvider<Node> storageProvider, IComparer<T> comparer) : base(storageProvider, comparer)
    {
    }

    public async ValueTask<IEnumerable<T>> GetRangeAsync(double from, double to)
    {
        await Semaphore.WaitAsync();
        try
        {
            return await GetRangeInternalAsync(RootId, from, to, new List<T>());
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private async ValueTask<IEnumerable<T>> GetRangeInternalAsync(long nodeId, double from, double to, List<T> items)
    {
        if(nodeId == NullId)
        {
            return items;
        }

        var node = await StorageProvider.ReadAsync(nodeId) ?? throw new InvalidOperationException("Node not found");

        if (node.Value.CompareTo(from) >= 0)
        {
            await GetRangeInternalAsync(node.LeftId, from, to, items);
        }

        if (node.Value.CompareTo(from) >= 0 && node.Value.CompareTo(to) <= 0)
        {
            items.Add(node.Value);
        }

        if (node.Value.CompareTo(to) <= 0)
        {
            await GetRangeInternalAsync(node.RightId, from, to, items);
        }

        return items;
    }
}
