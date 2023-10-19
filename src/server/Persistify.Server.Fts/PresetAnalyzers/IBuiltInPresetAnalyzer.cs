using Persistify.Server.Domain.PresetAnalyzers;

namespace Persistify.Server.Fts.PresetAnalyzers;

public interface IBuiltInPresetAnalyzer
{
    PresetAnalyzer GetPresetAnalyzer();
}
