using Persistify.Domain.Templates;
using Persistify.Helpers.Results;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface IAnalyzerFactory
{
    Result TryCreate(FullAnalyzerDescriptor descriptor, out IAnalyzer? analyzer);
    IAnalyzer Create(FullAnalyzerDescriptor descriptor);
    Result Validate(FullAnalyzerDescriptor descriptor);
}
