using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Persistify.Domain.Templates;

namespace Persistify.Client.Objects.Analyzers;

public class AnalyzerDescriptorStore : IAnalyzerDescriptorStore
{
    private readonly IEnumerable<IAnalyzerDescriptorFactory> _analyzerDescriptorFactories;
    private readonly ConcurrentDictionary<string, Analyzer> _analyzerDescriptors;

    public AnalyzerDescriptorStore(
        IEnumerable<IAnalyzerDescriptorFactory> analyzerDescriptorFactories
    )
    {
        _analyzerDescriptorFactories = analyzerDescriptorFactories;
        _analyzerDescriptors = new ConcurrentDictionary<string, Analyzer>();
    }

    public Analyzer Get(string name)
    {
        return _analyzerDescriptors.GetOrAdd(
            name,
            x => Create(x) ?? throw new InvalidOperationException($"Analyzer descriptor '{x}' not found.")
        );
    }

    private Analyzer? Create(string name)
    {
        foreach (var analyzerDescriptorFactory in _analyzerDescriptorFactories)
        {
            var analyzerDescriptor = analyzerDescriptorFactory.Create(name);
            if (analyzerDescriptor != null)
            {
                return analyzerDescriptor;
            }
        }

        return null;
    }
}
