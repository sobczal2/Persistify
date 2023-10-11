using System;
using System.Collections.Generic;
using System.Linq;
using Persistify.Domain.Fts;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.Analyzers;

public class StandardAnalyzer : IAnalyzer
{
    private readonly char[] _alphabet;
    private readonly IEnumerable<ITokenFilter> _tokenFilters;
    private readonly ITokenizer _tokenizer;

    public StandardAnalyzer(
        IEnumerable<ICharacterFilter> characterFilters,
        ITokenizer tokenizer,
        IEnumerable<ITokenFilter> tokenFilters
    )
    {
        _alphabet = characterFilters
            .SelectMany(x => x.AllowedCharacters)
            .Distinct()
            .ToArray();

        Array.Sort(_alphabet);

        _tokenizer = tokenizer;
        _tokenFilters = tokenFilters;
    }

    // TODO: Optimize this method
    public List<Token> Analyze(string text, AnalyzerMode mode)
    {
        var tokens = _tokenizer.Tokenize(text, _alphabet);

        foreach (var tokenFilter in _tokenFilters)
        {
            if (ShouldFilter(tokenFilter.Type, mode))
            {
                tokens = tokenFilter.Filter(tokens);
            }
        }

        foreach (var token in tokens)
        {
            token.Value = new string(token.Value
                .Where(x => Array.BinarySearch(_alphabet, x) >= 0)
                .ToArray()
            );
        }

        return tokens;
    }

    private bool ShouldFilter(TokenFilterType type, AnalyzerMode mode)
    {
        return type switch
        {
            TokenFilterType.IndexOnly => mode == AnalyzerMode.Index,
            TokenFilterType.SearchOnly => mode == AnalyzerMode.Search,
            TokenFilterType.IndexAndSearch => true,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public int AlphabetLength => _alphabet.Length;
}
