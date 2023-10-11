using System.Collections.Generic;
using Persistify.Domain.Fts;
using Persistify.Helpers.Strings;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.TokenFilters;

public class SuffixTokenFilter : ITokenFilter
{
    public List<Token> Filter(List<Token> tokens)
    {
        var filteredTokens = new List<Token>();

        foreach (var token in tokens)
        {
            var value = token.Value;
            var suffixes = StringHelpers.GetSuffixes(value);

            foreach (var suffix in suffixes)
            {
                if (suffix.Length == 0)
                {
                    continue;
                }

                filteredTokens.Add(new Token(suffix, token.Count, token.Positions, suffix.Length, token.Alphabet));
            }
        }

        return filteredTokens;
    }
}
