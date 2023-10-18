using Persistify.Domain.Templates;
using Persistify.Dtos.Templates.Common;
using Persistify.Helpers.Results;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface IAnalyzerFactory
{
    IAnalyzer Create(AnalyzerDescriptor descriptor);
    Result Validate(FullAnalyzerDescriptorDto descriptor);
}
