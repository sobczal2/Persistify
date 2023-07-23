using System.Collections.Concurrent;
using System.Linq;
using Persistify.Fts.Analysis.Abstractions;
using Persistify.Fts.Analysis.Exceptions;
using Persistify.Helpers.ErrorHandling;

namespace Persistify.Fts.Analysis.Presets;

public class StandardAnalyzerPresetFactory : IAnalyzerPresetFactory
{
    private static readonly ConcurrentBag<string> AvailablePresets = new() { "standard" };
    private readonly IAnalyzerFactory _analyzerFactory;

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
        var result = Validate(presetName);
        if (result.IsFailure)
        {
            analyzer = null;
            return result;
        }

        switch (presetName)
        {
            case "standard":
                analyzer = _analyzers.GetOrAdd(presetName,
                    static (_, analyzerFactory) => new StandardAnalyzerPreset().GetAnalyzer(analyzerFactory),
                    _analyzerFactory);
                return Result.Success;
            default:
                analyzer = null;
                return new UnsupportedPresetException(presetName, AvailablePresets);
        }
    }

    public Result Validate(string presetName)
    {
        if (!AvailablePresets.Contains(presetName))
        {
            return new UnsupportedPresetException(presetName, AvailablePresets);
        }

        return Result.Success;
    }
}
