using System.Collections.Generic;
using Persistify.Domain.Fts;
using Persistify.Helpers.Strings;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.TokenFilters;

public class SuffixTokenFilter : ITokenFilter
{
    public void Filter(List<Token> tokens)
    {
        var count = tokens.Count;
        for (var i = 0; i < count; i++)
        {
            var token = tokens[i];
            var value = token.Value;
            var suffixes = StringHelpers.GetNotEmptySuffixes(value);

            foreach (var suffix in suffixes)
            {
                var newPositions = new List<int>();

                foreach (var position in token.Positions)
                {
                    newPositions.Add(position + value.Length - suffix.Length);
                }

                tokens.Add(new Token(suffix, token.Count, newPositions, suffix.Length / (float)token.Value.Length,
                    token.Alphabet));
            }
        }
    }

    public TokenFilterType Type => TokenFilterType.IndexOnly;
}
