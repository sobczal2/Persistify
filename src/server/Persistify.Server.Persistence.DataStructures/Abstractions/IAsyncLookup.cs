using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.DataStructures.Abstractions;

public interface IAsyncLookup<TKey, TItem>
{
    ValueTask InitializeAsync();
    ValueTask AddAsync(TKey key, TItem item);
    ValueTask<List<TItem>> GetAsync(TKey key);
    ValueTask<List<TItem>> GetRangeAsync(TKey from, TKey to);
    ValueTask RemoveAsync(TKey key, TItem item);
}
