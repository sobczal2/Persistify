using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.TokenFilters;

public class LowercaseTokenFilter : ITokenFilter
{
    private static void Filter(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            token.Term = token.Term.ToLowerInvariant();
        }
    }

    public void FilterForSearch(IEnumerable<SearchToken> tokens)
    {
        Filter(tokens);
    }

    public void FilterForIndex(IEnumerable<IndexToken> tokens)
    {
        Filter(tokens);
    }
}
