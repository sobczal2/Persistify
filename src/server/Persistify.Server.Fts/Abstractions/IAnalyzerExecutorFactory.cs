using Persistify.Domain.Templates;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Common;
using Persistify.Helpers.Results;

namespace Persistify.Server.Fts.Abstractions;

public interface IAnalyzerExecutorFactory
{
    IAnalyzerExecutor Create(Analyzer descriptor);
    Result Validate(FullAnalyzerDto descriptor);
}
