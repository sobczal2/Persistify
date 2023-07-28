using System;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.DataStructures.Abstractions;

public interface IAsyncLookup<TKey, TItem>
{
    ValueTask InitializeAsync();
    ValueTask AddAsync(TKey key, TItem item);
    ValueTask<TItem> GetAsync(TKey key);
}
