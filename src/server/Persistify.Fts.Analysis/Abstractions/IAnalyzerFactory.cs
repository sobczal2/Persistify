using Persistify.Domain.Templates;
using Persistify.Helpers.ErrorHandling;

namespace Persistify.Fts.Analysis.Abstractions;

public interface IAnalyzerFactory
{
    Result TryCreate(AnalyzerDescriptor descriptor, out IAnalyzer? analyzer);
    IAnalyzer Create(AnalyzerDescriptor descriptor);
    Result Validate(AnalyzerDescriptor descriptor);
}
