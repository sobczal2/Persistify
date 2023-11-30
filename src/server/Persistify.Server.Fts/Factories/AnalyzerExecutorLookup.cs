using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Helpers.Results;
using Persistify.Server.Domain.Templates;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Analyzers;
using Persistify.Server.Fts.CharacterSets;
using Persistify.Server.Fts.TokenFilters;
using Persistify.Server.Fts.Tokenizers;

namespace Persistify.Server.Fts.Factories;

public class AnalyzerExecutorLookup : IAnalyzerExecutorLookup
{
    private static readonly ConcurrentDictionary<string, ICharacterFilter> CharacterFilters;

    private static readonly ConcurrentDictionary<string, ICharacterSet> CharacterSets;

    private static readonly ConcurrentDictionary<string, ITokenizer> Tokenizers;

    private static readonly ConcurrentDictionary<string, ITokenFilter> TokenFilters;

    static AnalyzerExecutorLookup()
    {
        CharacterFilters = new ConcurrentDictionary<string, ICharacterFilter>(
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces().Contains(typeof(ICharacterFilter)))
                .Select(Activator.CreateInstance)
                .Cast<ICharacterFilter>()
                .ToDictionary(c => c.Code)
        );

        CharacterSets = new ConcurrentDictionary<string, ICharacterSet>(
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces().Contains(typeof(ICharacterSet)))
                .Select(Activator.CreateInstance)
                .Cast<ICharacterSet>()
                .ToDictionary(c => c.Code)
        );

        Tokenizers = new ConcurrentDictionary<string, ITokenizer>(
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces().Contains(typeof(ITokenizer)))
                .Select(Activator.CreateInstance)
                .Cast<ITokenizer>()
                .ToDictionary(c => c.Code)
        );

        TokenFilters = new ConcurrentDictionary<string, ITokenFilter>(
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces().Contains(typeof(ITokenFilter)))
                .Select(Activator.CreateInstance)
                .Cast<ITokenFilter>()
                .ToDictionary(c => c.Code)
        );
    }

    public Result Validate(FullAnalyzerDto analyzerDto)
    {
        foreach (var characterFilter in analyzerDto.CharacterFilterNames)
        {
            if (!CharacterFilters.ContainsKey(characterFilter))
            {
                return new DynamicValidationPersistifyException(
                    nameof(FullAnalyzerDto),
                    $"Character filter '{characterFilter}' is not supported. Supported character filters: {string.Join(", ", CharacterFilters.Keys)}"
                );
            }
        }

        foreach (var characterSetName in analyzerDto.CharacterSetNames)
        {
            if (!CharacterSets.ContainsKey(characterSetName))
            {
                return new DynamicValidationPersistifyException(
                    nameof(FullAnalyzerDto),
                    $"Character set '{characterSetName}' is not supported. Supported character sets: {string.Join(", ", CharacterSets.Keys)}"
                );
            }
        }

        if (!Tokenizers.ContainsKey(analyzerDto.TokenizerName))
        {
            return new DynamicValidationPersistifyException(
                nameof(FullAnalyzerDto),
                $"Tokenizer '{analyzerDto.TokenizerName}' is not supported. Supported tokenizers: {string.Join(", ", Tokenizers.Keys)}"
            );
        }

        foreach (var tokenFilter in analyzerDto.TokenFilterNames)
        {
            if (!TokenFilters.ContainsKey(tokenFilter))
            {
                return new DynamicValidationPersistifyException(
                    nameof(FullAnalyzerDto),
                    $"Token filter '{tokenFilter}' is not supported. Supported token filters: {string.Join(", ", TokenFilters.Keys)}"
                );
            }
        }

        return Result.Ok;
    }

    public IAnalyzerExecutor Create(Analyzer analyzer)
    {
        var characterFilters = analyzer.CharacterFilterNames.Select(GetCharacterFilter).ToList();
        var characterSets = analyzer.CharacterSetNames.Select(GetCharacterSet).ToList();
        var tokenizer = GetTokenizer(analyzer.TokenizerName);
        var tokenFilters = analyzer.TokenFilterNames.Select(GetTokenFilter).ToList();

        return new AnalyzerExecutor(characterFilters, characterSets, tokenizer, tokenFilters);
    }

    private static ICharacterFilter GetCharacterFilter(string name)
    {
        return CharacterFilters.TryGetValue(name, out var characterFilter)
            ? characterFilter
            : throw new NotImplementedException();
    }

    private static ICharacterSet GetCharacterSet(string name)
    {
        return CharacterSets.TryGetValue(name, out var characterSet)
            ? characterSet
            : throw new NotImplementedException();
    }

    private static ITokenizer GetTokenizer(string name)
    {
        return Tokenizers.TryGetValue(name, out var tokenizer)
            ? tokenizer
            : throw new NotImplementedException();
    }

    private static ITokenFilter GetTokenFilter(string name)
    {
        return TokenFilters.TryGetValue(name, out var tokenFilter)
            ? tokenFilter
            : throw new NotImplementedException();
    }
}
