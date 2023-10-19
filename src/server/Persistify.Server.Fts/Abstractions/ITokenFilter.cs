using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Abstractions;

public interface ITokenFilter
{
    void FilterForSearch(IEnumerable<SearchToken> tokens);
    void FilterForIndex(IEnumerable<IndexToken> tokens);
}
