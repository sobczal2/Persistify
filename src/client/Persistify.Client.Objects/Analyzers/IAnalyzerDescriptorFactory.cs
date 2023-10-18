using Persistify.Domain.Templates;

namespace Persistify.Client.Objects.Analyzers;

public interface IAnalyzerDescriptorFactory
{
    Analyzer? Create(string name);
}
