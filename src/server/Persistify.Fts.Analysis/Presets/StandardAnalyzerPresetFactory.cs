using System.Collections.Concurrent;
using Persistify.Fts.Analysis.Abstractions;
using Persistify.Fts.Analysis.Analyzers;
using Persistify.Fts.Analysis.Exceptions;
using Persistify.Helpers.ErrorHandling;

namespace Persistify.Fts.Analysis.Presets;

public class StandardAnalyzerPresetFactory : IAnalyzerPresetFactory
{
    private readonly IAnalyzerFactory _analyzerFactory;
    private static readonly ConcurrentBag<string> AvailablePresets = new() { "standard" };

    private readonly ConcurrentDictionary<string, IAnalyzer> _analyzers;

    public StandardAnalyzerPresetFactory(
        IAnalyzerFactory analyzerFactory
    )
    {
        _analyzerFactory = analyzerFactory;
        _analyzers = new ConcurrentDictionary<string, IAnalyzer>();
    }

    public Result TryCreate(string presetName, out IAnalyzer? analyzer)
    {
        if (!AvailablePresets.Contains(presetName))
        {
            analyzer = null;
            return new UnsupportedPresetException(presetName, AvailablePresets);
        }

        switch (presetName)
        {
            case "standard":
                analyzer = _analyzers.GetOrAdd(presetName,
                    (_, analyzerFactory) => new StandardAnalyzerPreset().GetAnalyzer(analyzerFactory),
                    _analyzerFactory);
                return Result.Success;
            default:
                analyzer = null;
                return new UnsupportedPresetException(presetName, AvailablePresets);
        }
    }
}
