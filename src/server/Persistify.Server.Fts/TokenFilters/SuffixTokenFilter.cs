using System;
using System.Collections.Generic;
using System.Linq;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.TokenFilters;

public class SuffixTokenFilter : ITokenFilter
{
    public string Code => "suffix";

    public void FilterForSearch(
        List<SearchToken> tokens
    )
    {
    }

    public void FilterForIndex(
        List<IndexToken> tokens
    )
    {
        var initialTokensCount = tokens.Count;
        for (var i = 0; i < initialTokensCount; i++)
        {
            var token = tokens[i];
            var term = token.Term.AsSpan();

            for (var j = 1; j < term.Length; j++)
            {
                var suffixSpan = term.Slice(j);
                var suffix = suffixSpan.ToString();

                var charsCut = j;
                tokens.Add(
                    new IndexToken(
                        suffix,
                        token.Alphabet,
                        token.DocumentPositions.Select(
                            x =>
                                new DocumentPosition(
                                    x.DocumentId,
                                    x.Position + charsCut,
                                    (token.Term.Length - charsCut) / (float)token.Term.Length
                                )
                        )
                    )
                );
            }
        }
    }
}
