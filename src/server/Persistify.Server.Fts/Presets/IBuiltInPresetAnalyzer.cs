using Persistify.Domain.PresetAnalyzerDescriptors;

namespace Persistify.Server.Fts.Presets;

public interface IBuiltInPresetAnalyzer
{
    PresetAnalyzer GetPresetAnalyzer();
}
