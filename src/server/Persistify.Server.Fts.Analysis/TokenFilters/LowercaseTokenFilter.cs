using System.Collections.Generic;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Fts.Analysis.Tokens;

namespace Persistify.Server.Fts.Analysis.TokenFilters;

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
