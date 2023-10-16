using System.Collections.Generic;
using Persistify.Domain.Fts;
using Persistify.Helpers.Strings;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.TokenFilters;

public class SuffixTokenFilter : ITokenFilter
{
    public IEnumerable<Token> Filter(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            var value = token.Value;
            var suffixes = StringHelpers.GetNotEmptySuffixes(value);

            foreach (var suffix in suffixes)
            {
                if (suffix.Length == 0)
                {
                    continue;
                }

                var newPositions = new List<int>();

                foreach (var position in token.Positions)
                {
                    newPositions.Add(position + value.Length - suffix.Length);
                }

                yield return new Token(suffix, token.Count, newPositions, suffix.Length / (float)token.Value.Length,
                    token.Alphabet);
            }
        }
    }

    public TokenFilterType Type => TokenFilterType.IndexOnly;
}
