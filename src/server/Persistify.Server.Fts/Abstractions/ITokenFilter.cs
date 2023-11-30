using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Abstractions;

public interface ITokenFilter
{
    string Code { get; }
    void FilterForSearch(List<SearchToken> tokens);
    void FilterForIndex(List<IndexToken> tokens);
}
