using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Tokens;

namespace Persistify.Indexes.Common;

public interface IIndexer<TValue>
    where TValue : notnull
{
    Task IndexAsync(long id, Token<TValue> token, string typeName);
    Task IndexAsync(long id, IEnumerable<Token<TValue>> tokens, string typeName);
    Task<IEnumerable<Index>> SearchAsync(ISearchPredicate searchPredicate, string typeName);
    Task<int> RemoveAsync(long id, string typeName);
}