using System.Collections.Generic;
using Persistify.Domain.Templates;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.Presets;

public class StandardAnalyzerPreset : IAnalyzerPreset
{
    private const string TokenizerName = "whitespace";

    private static readonly IEnumerable<string> CharacterFilterNames =
        new List<string> { "lowercase_letters", "uppercase_letters", "digits" };

    private static readonly IEnumerable<string> TokenFilterNames = new List<string> { "lowercase", "suffix" };

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
