using Persistify.Domain.PresetAnalyzers;
using Persistify.Dtos.PresetAnalyzers;

namespace Persistify.Dtos.Mappers;

public static class PresetAnalyzerMapper
{
    public static PresetAnalyzerDto Map(PresetAnalyzer presetAnalyzer)
    {
        return new PresetAnalyzerDto
        {
            Id = presetAnalyzer.Id,
            PresetAnalyzerName = presetAnalyzer.Name,
            FullAnalyzerDto = FullAnalyzerMapper.Map(presetAnalyzer.Analyzer),
        };
    }
}
