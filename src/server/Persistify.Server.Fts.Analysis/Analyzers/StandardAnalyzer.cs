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
    public List<Token> Analyze(string text)
    {
        var filteredText = new string(text
            .Where(x => _alphabet.Contains(x))
            .ToArray());

        var tokens = _tokenizer.Tokenize(filteredText, _alphabet);

        foreach (var tokenFilter in _tokenFilters)
        {
            tokens = tokenFilter.Filter(tokens);
        }

        return tokens;
    }

    public int AlphabetLength => _alphabet.Length;
}
