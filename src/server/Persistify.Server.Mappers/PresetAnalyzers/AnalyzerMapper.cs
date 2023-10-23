using Persistify.Dtos.PresetAnalyzers;
using Persistify.Server.Domain.Templates;

namespace Persistify.Server.Mappers.PresetAnalyzers;

public static class AnalyzerMapper
{
    public static FullAnalyzerDto ToDto(this Analyzer analyzer)
    {
        return new FullAnalyzerDto
        {
            CharacterSetNames = analyzer.CharacterSetNames,
            CharacterFilterNames = analyzer.CharacterFilterNames,
            TokenizerName = analyzer.TokenizerName,
            TokenFilterNames = analyzer.TokenFilterNames
        };
    }

    public static Analyzer ToDomain(this FullAnalyzerDto fullAnalyzerDto)
    {
        return new Analyzer
        {
            CharacterFilterNames = fullAnalyzerDto.CharacterFilterNames,
            CharacterSetNames = fullAnalyzerDto.CharacterSetNames,
            TokenizerName = fullAnalyzerDto.TokenizerName,
            TokenFilterNames = fullAnalyzerDto.TokenFilterNames
        };
    }
}
