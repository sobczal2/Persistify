using Persistify.Domain.Templates;

namespace Persistify.Client.Objects.Analyzers;

public class DefaultAnalyzerDescriptorFactory : IAnalyzerDescriptorFactory
{
    public AnalyzerDescriptor? Create(string name)
    {
        return name switch
        {
            PresetAnalyzerDescriptors.Standard => new PresetAnalyzerDescriptor { PresetName = "standard", },
            _ => null,
        };
    }
}
