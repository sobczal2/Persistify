using Persistify.Domain.Fts;
using Persistify.Fts.Analysis.Abstractions;

namespace Persistify.Fts.Analysis.TokenFilters;

public class LowercaseTokenFilter : ITokenFilter
{
    public List<Token> Filter(List<Token> tokens)
    {
        foreach (var token in tokens)
        {
            token.Value = token.Value.ToLowerInvariant();
        }

        return tokens;
    }
}
