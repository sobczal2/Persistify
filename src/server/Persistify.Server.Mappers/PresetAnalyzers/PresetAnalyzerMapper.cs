using Persistify.Dtos.PresetAnalyzers;
using Persistify.Server.Domain.PresetAnalyzers;

namespace Persistify.Server.Mappers.PresetAnalyzers;

public static class PresetAnalyzerMapper
{
    public static PresetAnalyzerDto ToDto(this PresetAnalyzer presetAnalyzer)
    {
        return new()
        {
            Name = presetAnalyzer.Name,
            FullAnalyzerDto = presetAnalyzer.Analyzer.ToDto()
        };
    }
}
