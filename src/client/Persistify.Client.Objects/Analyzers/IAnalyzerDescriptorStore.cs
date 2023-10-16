using Persistify.Domain.Templates;

namespace Persistify.Client.Objects.Analyzers;

public interface IAnalyzerDescriptorStore
{
    AnalyzerDescriptor Get(string name);
}
