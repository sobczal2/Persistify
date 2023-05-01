using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Indexes.Common;
using Persistify.Storage;
using Persistify.Tokens;
using Index = Persistify.Indexes.Common.Index;

namespace Persistify.Indexes.Boolean;

public class BooleanIndexer : IIndexer<bool>, IPersisted
{
    public Task IndexAsync(long id, Token<bool> token, string typeName)
    {
        return Task.CompletedTask;
    }

    public Task IndexAsync(long id, IEnumerable<Token<bool>> tokens, string typeName)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Index>> SearchAsync(ISearchPredicate searchPredicate, string typeName)
    {
        return Task.FromResult(Enumerable.Empty<Index>());
    }

    public Task<int> RemoveAsync(long id, string typeName)
    {
        return Task.FromResult(0);
    }

    public ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }
}