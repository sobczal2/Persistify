using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.TokenFilters;

public class LowercaseTokenFilter : ITokenFilter
{
    public List<SearchToken> FilterForSearch(List<SearchToken> tokens)
    {
        Filter(tokens);

        return tokens;
    }

    public List<IndexToken> FilterForIndex(List<IndexToken> tokens)
    {
        Filter(tokens);

        return tokens;
    }

    private static void Filter(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            token.Term = token.Term.ToLowerInvariant();
        }
    }
}
