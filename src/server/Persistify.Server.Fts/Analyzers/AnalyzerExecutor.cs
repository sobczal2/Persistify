using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Analyzers;

public class AnalyzerExecutor : IAnalyzerExecutor
{
    private readonly char[] _alphabet;
    private readonly IEnumerable<ITokenFilter> _tokenFilters;
    private readonly ITokenizer _tokenizer;

    public AnalyzerExecutor(
        IEnumerable<ICharacterSet> characterFilters,
        ITokenizer tokenizer,
        IEnumerable<ITokenFilter> tokenFilters
    )
    {
        _alphabet = characterFilters
            .SelectMany(x => x.Characters)
            .Distinct()
            .ToArray();

        Array.Sort(_alphabet);

        _tokenizer = tokenizer;
        _tokenFilters = tokenFilters;
    }

    public int AlphabetLength => _alphabet.Length;
    public IEnumerable<SearchToken> AnalyzeForSearch(string input)
    {
        var tokens = _tokenizer.TokenizeForSearch(input, _alphabet).ToList();

        foreach (var tokenFilter in _tokenFilters)
        {
            tokenFilter.FilterForSearch(tokens);
        }

        RemoveNonAlphabetCharacters(tokens);

        return tokens;
    }

    public IEnumerable<IndexToken> AnalyzeForIndex(string text, int documentId)
    {
        var tokens = _tokenizer.TokenizeForIndex(text, _alphabet, documentId).ToList();

        foreach (var tokenFilter in _tokenFilters)
        {
            tokenFilter.FilterForIndex(tokens);
        }

        RemoveNonAlphabetCharacters(tokens);

        return tokens;
    }

    private void RemoveNonAlphabetCharacters(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            token.Term = new string(token.Term
                .Where(x => Array.BinarySearch(_alphabet, x) >= 0)
                .ToArray()
            );
        }
    }
}
