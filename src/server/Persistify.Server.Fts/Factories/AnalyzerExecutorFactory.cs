using System;
using System.Collections.Concurrent;
using System.Linq;
using Persistify.Server.Domain.Templates;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Common;
using Persistify.Helpers.Results;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Analyzers;
using Persistify.Server.Fts.CharacterSets;
using Persistify.Server.Fts.Exceptions;
using Persistify.Server.Fts.TokenFilters;
using Persistify.Server.Fts.Tokenizers;

namespace Persistify.Server.Fts.Factories;

public class AnalyzerExecutorFactory : IAnalyzerExecutorFactory
{
    private static readonly ConcurrentBag<string> SupportedCharacterFilters =
        new() { "lowercase_letters", "uppercase_letters", "digits" };

    private static readonly ConcurrentBag<string> SupportedTokenizers = new() { "standard", "whitespace" };
    private static readonly ConcurrentBag<string> SupportedTokenFilters = new() { "lowercase", "suffix" };

    public Result Validate(FullAnalyzerDto descriptor)
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

    public IAnalyzerExecutor Create(Analyzer descriptor)
    {
        var characterFilters = descriptor.CharacterFilterNames.Select(CreateCharacterFilter).ToList();
        var tokenizer = CreateTokenizer(descriptor.TokenizerName);
        var tokenFilters = descriptor.TokenFilterNames.Select(CreateTokenFilter).ToList();

        return new AnalyzerExecutor(characterFilters, tokenizer, tokenFilters);
    }

    private ICharacterSet CreateCharacterFilter(string name)
    {
        return name switch
        {
            "lowercase_letters" => new LowercaseLettersCharacterSet(),
            "uppercase_letters" => new UppercaseLettersCharacterSet(),
            "digits" => new DigitsCharacterSet(),
            _ => throw new NotImplementedException()
        };
    }

    private ITokenizer CreateTokenizer(string name)
    {
        return name switch
        {
            "whitespace" => new WhitespaceTokenizer(),
            _ => throw new NotImplementedException()
        };
    }

    private ITokenFilter CreateTokenFilter(string name)
    {
        return name switch
        {
            "lowercase" => new LowercaseTokenFilter(),
            "suffix" => new SuffixTokenFilter(),
            _ => throw new NotImplementedException()
        };
    }
}
