using Persistify.Domain.Templates;

namespace Persistify.Client.Objects.Analyzers;

public interface IAnalyzerDescriptorFactory
{
    AnalyzerDescriptor? Create(string name);
}
