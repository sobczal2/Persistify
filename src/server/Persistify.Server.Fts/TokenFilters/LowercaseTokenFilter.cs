using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.TokenFilters;

public class LowercaseTokenFilter : ITokenFilter
{
    public string Code => "lowercase";

    public void FilterForSearch(
        List<SearchToken> tokens
    )
    {
        Filter(tokens);
    }

    public void FilterForIndex(
        List<IndexToken> tokens
    )
    {
        Filter(tokens);
    }

    private static void Filter(
        IEnumerable<Token> tokens
    )
    {
        foreach (var token in tokens)
        {
            token.Term = token.Term.ToLowerInvariant();
        }
    }
}
