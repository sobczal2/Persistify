using Persistify.Domain.PresetAnalyzers;

namespace Persistify.Server.Fts.Presets;

public interface IBuiltInPresetAnalyzer
{
    PresetAnalyzer GetPresetAnalyzer();
}
