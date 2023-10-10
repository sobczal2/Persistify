using Persistify.Helpers.Results;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface IAnalyzerPresetFactory
{
    Result TryCreate(string presetName, out IAnalyzer? analyzer);
    Result Validate(string presetName);
}
