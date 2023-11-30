using Persistify.Dtos.PresetAnalyzers;
using Persistify.Helpers.Results;
using Persistify.Server.Domain.Templates;

namespace Persistify.Server.Fts.Abstractions;

public interface IAnalyzerExecutorLookup
{
    IAnalyzerExecutor Create(
        Analyzer analyzer
    );

    Result Validate(
        FullAnalyzerDto analyzerDto
    );
}
