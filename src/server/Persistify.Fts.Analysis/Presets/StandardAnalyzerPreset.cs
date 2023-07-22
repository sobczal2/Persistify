using Persistify.Domain.Templates;
using Persistify.Fts.Analysis.Abstractions;

namespace Persistify.Fts.Analysis.Presets;

public class StandardAnalyzerPreset : IAnalyzerPreset
{
    private static readonly List<string> CharacterFilterNames = new();
    private static readonly string TokenizerName = "standard";
    private static readonly List<string> TokenFilterNames = new() { "lowercase" };

    public IAnalyzer GetAnalyzer(IAnalyzerFactory analyzerFactory)
    {
        return analyzerFactory.Create(
            new AnalyzerDescriptor
            {
                CharacterFilterNames = CharacterFilterNames,
                TokenizerName = TokenizerName,
                TokenFilterNames = TokenFilterNames
            }
        );
    }
}
