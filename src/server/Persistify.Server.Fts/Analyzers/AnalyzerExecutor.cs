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
    private readonly IEnumerable<ICharacterFilter> _characterFilters;
    private readonly ITokenizer _tokenizer;

    public AnalyzerExecutor(
        IEnumerable<ICharacterFilter> characterFilters,
        IEnumerable<ICharacterSet> characterSets,
        ITokenizer tokenizer,
        IEnumerable<ITokenFilter> tokenFilters
    )
    {
        _alphabet = characterSets
            .SelectMany(x => x.Characters)
            .Distinct()
            .ToArray();

        Array.Sort(_alphabet);

        _characterFilters = characterFilters;
        _tokenizer = tokenizer;
        _tokenFilters = tokenFilters;
    }

    public int AlphabetLength => _alphabet.Length;
    public IEnumerable<SearchToken> AnalyzeForSearch(string input)
    {
        foreach (var characterFilter in _characterFilters)
        {
            input = characterFilter.Filter(input);
        }

        var tokens = _tokenizer.TokenizeForSearch(input, _alphabet).ToList();

        foreach (var tokenFilter in _tokenFilters)
        {
            tokenFilter.FilterForSearch(tokens);
        }

        RemoveNonAlphabetCharactersForSearch(tokens);

        return tokens;
    }

    public IEnumerable<IndexToken> AnalyzeForIndex(string text, int documentId)
    {
        foreach (var characterFilter in _characterFilters)
        {
            text = characterFilter.Filter(text);
        }

        var tokens = _tokenizer.TokenizeForIndex(text, _alphabet, documentId).ToList();

        foreach (var tokenFilter in _tokenFilters)
        {
            tokenFilter.FilterForIndex(tokens);
        }

        RemoveNonAlphabetCharactersForIndex(tokens);

        return tokens;
    }

    private void RemoveNonAlphabetCharactersForIndex(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            token.Term = new string(token.Term
                .Where(x => Array.BinarySearch(_alphabet, x) >= 0)
                .ToArray()
            );
        }
    }

    private void RemoveNonAlphabetCharactersForSearch(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            token.Term = new string(token.Term
                .Where(x => Array.BinarySearch(_alphabet, x) >= 0 || x == '?' || x == '*')
                .ToArray()
            );
        }
    }
}
