using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.TokenFilters;

public class LowercaseTokenFilter : ITokenFilter
{
    public void Filter(List<Token> tokens)
    {
        foreach (var token in tokens)
        {
            token.Value = token.Value.ToLowerInvariant();
        }
    }

    public TokenFilterType Type => TokenFilterType.IndexAndSearch;
}
