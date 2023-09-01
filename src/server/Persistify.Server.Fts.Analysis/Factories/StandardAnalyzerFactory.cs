using System;
using System.Collections.Concurrent;
using System.Linq;
using Persistify.Domain.Templates;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Fts.Analysis.Analyzers;
using Persistify.Server.Fts.Analysis.Exceptions;
using Persistify.Server.Fts.Analysis.TokenFilters;
using Persistify.Server.Fts.Analysis.Tokenizers;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Fts.Analysis.Factories;

public class StandardAnalyzerFactory : IAnalyzerFactory
{
    private static readonly ConcurrentBag<string> SupportedCharacterFilters = new();
    private static readonly ConcurrentBag<string> SupportedTokenizers = new() { "standard", "whitespace" };
    private static readonly ConcurrentBag<string> SupportedTokenFilters = new() { "lowercase" };

    public Result TryCreate(AnalyzerDescriptor descriptor, out IAnalyzer? analyzer)
    {
        var result = Validate(descriptor);
        if (result.Failure)
        {
            analyzer = null;
            return result;
        }

        analyzer = Create(descriptor);

        return Result.Ok;
    }

    public Result Validate(AnalyzerDescriptor descriptor)
    {
        foreach (var characterFilter in descriptor.CharacterFilterNames)
        {
            if (!SupportedCharacterFilters.Contains(characterFilter))
            {
                return new UnsupportedCharacterFilterException(characterFilter, SupportedCharacterFilters);
            }
        }

        if (!SupportedTokenizers.Contains(descriptor.TokenizerName))
        {
            return new UnsupportedTokenizerException(descriptor.TokenizerName, SupportedTokenizers);
        }

        foreach (var tokenFilter in descriptor.TokenFilterNames)
        {
            if (!SupportedTokenFilters.Contains(tokenFilter))
            {
                return new UnsupportedTokenFilterException(tokenFilter, SupportedTokenFilters);
            }
        }

        return Result.Ok;
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
