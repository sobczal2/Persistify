using Persistify.Helpers.ErrorHandling;

namespace Persistify.Fts.Analysis.Abstractions;

public interface IAnalyzerPresetFactory
{
    Result TryCreate(string presetName, out IAnalyzer? analyzer);
    Result Validate(string presetName);
}
