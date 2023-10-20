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
        new() { };
    private static readonly ConcurrentBag<string> SupportedCharacterSets =
        new() { "lowercase_letters", "uppercase_letters", "digits" };

    private static readonly ConcurrentBag<string> SupportedTokenizers = new() { "standard", "whitespace" };
    private static readonly ConcurrentBag<string> SupportedTokenFilters = new() { "lowercase", "suffix" };

    public Result Validate(FullAnalyzerDto analyzerDto)
    {
        foreach (var characterFilter in analyzerDto.CharacterFilterNames)
        {
            if (!SupportedCharacterFilters.Contains(characterFilter))
            {
                return new UnsupportedCharacterFilterException(characterFilter, SupportedCharacterFilters);
            }
        }

        foreach (var characterSetName in analyzerDto.CharacterSetNames)
        {
            if (!SupportedCharacterSets.Contains(characterSetName))
            {
                return new UnsupportedCharacterSetException(characterSetName, SupportedCharacterSets);
            }
        }

        if (!SupportedTokenizers.Contains(analyzerDto.TokenizerName))
        {
            return new UnsupportedTokenizerException(analyzerDto.TokenizerName, SupportedTokenizers);
        }

        foreach (var tokenFilter in analyzerDto.TokenFilterNames)
        {
            if (!SupportedTokenFilters.Contains(tokenFilter))
            {
                return new UnsupportedTokenFilterException(tokenFilter, SupportedTokenFilters);
            }
        }

        return Result.Ok;
    }

    public IAnalyzerExecutor Create(Analyzer analyzer)
    {
        var characterFilters = analyzer.CharacterFilterNames.Select(CreateCharacterFilter).ToList();
        var characterSets = analyzer.CharacterSetNames.Select(CreateCharacterSet).ToList();
        var tokenizer = CreateTokenizer(analyzer.TokenizerName);
        var tokenFilters = analyzer.TokenFilterNames.Select(CreateTokenFilter).ToList();

        return new AnalyzerExecutor(characterFilters, characterSets, tokenizer, tokenFilters);
    }

    private static ICharacterFilter CreateCharacterFilter(string name)
    {
        return name switch
        {
            _ => throw new NotImplementedException()
        };
    }

    private static ICharacterSet CreateCharacterSet(string name)
    {
        return name switch
        {
            "lowercase_letters" => new LowercaseLettersCharacterSet(),
            "uppercase_letters" => new UppercaseLettersCharacterSet(),
            "digits" => new DigitsCharacterSet(),
            _ => throw new NotImplementedException()
        };
    }

    private static ITokenizer CreateTokenizer(string name)
    {
        return name switch
        {
            "whitespace" => new WhitespaceTokenizer(),
            _ => throw new NotImplementedException()
        };
    }

    private static ITokenFilter CreateTokenFilter(string name)
    {
        return name switch
        {
            "lowercase" => new LowercaseTokenFilter(),
            "suffix" => new SuffixTokenFilter(),
            _ => throw new NotImplementedException()
        };
    }
}
