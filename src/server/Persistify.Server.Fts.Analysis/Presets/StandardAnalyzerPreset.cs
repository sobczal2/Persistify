using System.Collections.Generic;
using Persistify.Domain.Templates;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.Presets;

public class StandardAnalyzerPreset : IAnalyzerPreset
{
    private const string TokenizerName = "standard";
    private static readonly List<string> CharacterFilterNames = new();
    private static readonly List<string> TokenFilterNames = new() { "lowercase" };

    public IAnalyzer GetAnalyzer(IAnalyzerFactory analyzerFactory)
    {
        return analyzerFactory.Create(
            new FullAnalyzerDescriptor
            {
                CharacterFilterNames = CharacterFilterNames,
                TokenizerName = TokenizerName,
                TokenFilterNames = TokenFilterNames
            }
        );
    }
}
