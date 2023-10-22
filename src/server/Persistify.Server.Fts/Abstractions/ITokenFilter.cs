using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Abstractions;

public interface ITokenFilter
{
    List<SearchToken> FilterForSearch(List<SearchToken> tokens);
    List<IndexToken> FilterForIndex(List<IndexToken> tokens);
}
