using Persistify.Server.Domain.Templates;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Common;
using Persistify.Helpers.Results;

namespace Persistify.Server.Fts.Abstractions;

public interface IAnalyzerExecutorFactory
{
    IAnalyzerExecutor Create(Analyzer analyzer);
    Result Validate(FullAnalyzerDto analyzerDto);
}
