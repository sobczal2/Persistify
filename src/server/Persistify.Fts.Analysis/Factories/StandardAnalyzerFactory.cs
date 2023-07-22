using System;
using System.Collections.Concurrent;
using System.Linq;
using Persistify.Domain.Templates;
using Persistify.Fts.Analysis.Abstractions;
using Persistify.Fts.Analysis.Analyzers;
using Persistify.Fts.Analysis.Exceptions;
using Persistify.Fts.Analysis.TokenFilters;
using Persistify.Fts.Analysis.Tokenizers;
using Persistify.Helpers.ErrorHandling;

namespace Persistify.Fts.Analysis.Factories;

public class StandardAnalyzerFactory : IAnalyzerFactory
{
    private static readonly ConcurrentBag<string> SupportedCharacterFilters = new();
    private static readonly ConcurrentBag<string> SupportedTokenizers = new() { "standard", "whitespace" };
    private static readonly ConcurrentBag<string> SupportedTokenFilters = new() { "lowercase" };

    public Result TryCreate(AnalyzerDescriptor descriptor, out IAnalyzer? analyzer)
    {
        foreach (var characterFilter in descriptor.CharacterFilterNames)
        {
            if (!SupportedCharacterFilters.Contains(characterFilter))
            {
                analyzer = null;
                return new UnsupportedCharacterFilterException(characterFilter, SupportedCharacterFilters);
            }
        }

        if (!SupportedTokenizers.Contains(descriptor.TokenizerName))
        {
            analyzer = null;
            return new UnsupportedTokenizerException(descriptor.TokenizerName, SupportedTokenizers);
        }

        foreach (var tokenFilter in descriptor.TokenFilterNames)
        {
            if (!SupportedTokenFilters.Contains(tokenFilter))
            {
                analyzer = null;
                return new UnsupportedTokenFilterException(tokenFilter, SupportedTokenFilters);
            }
        }

        analyzer = Create(descriptor);

        return Result.Success;
    }

    public IAnalyzer Create(AnalyzerDescriptor descriptor)
    {
        var characterFilters = descriptor.CharacterFilterNames.Select(CreateCharacterFilter).ToList();
        var tokenizer = CreateTokenizer(descriptor.TokenizerName);
        var tokenFilters = descriptor.TokenFilterNames.Select(CreateTokenFilter).ToList();

        return new StandardAnalyzer(characterFilters, tokenizer, tokenFilters);
    }

    private ICharacterFilter CreateCharacterFilter(string name)
    {
        return name switch
        {
            _ => throw new NotImplementedException()
        };
    }

    private ITokenizer CreateTokenizer(string name)
    {
        return name switch
        {
            "standard" => new StandardTokenizer(),
            "whitespace" => new WhitespaceTokenizer(),
            _ => throw new NotImplementedException()
        };
    }

    private ITokenFilter CreateTokenFilter(string name)
    {
        return name switch
        {
            "lowercase" => new LowercaseTokenFilter(),
            _ => throw new NotImplementedException()
        };
    }
}
