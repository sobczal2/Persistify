using Persistify.Domain.Templates;
using Persistify.Dtos.PresetAnalyzers;

namespace Persistify.Dtos.Mappers;

public static class FullAnalyzerMapper
{
    public static FullAnalyzerDto Map(Analyzer analyzer)
    {
        return new FullAnalyzerDto
        {
            CharacterFilterNames = analyzer.CharacterFilterNames,
            TokenizerName = analyzer.TokenizerName,
            TokenFilterNames = analyzer.TokenFilterNames,
        };
    }
}
