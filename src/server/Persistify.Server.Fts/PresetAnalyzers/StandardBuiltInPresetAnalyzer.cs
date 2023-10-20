using System.Collections.Generic;
using Persistify.Server.Domain.PresetAnalyzers;
using Persistify.Server.Domain.Templates;

namespace Persistify.Server.Fts.PresetAnalyzers;

public class StandardBuiltInPresetAnalyzer : IBuiltInPresetAnalyzer
{
    private const string TokenizerName = "whitespace";

    private static readonly List<string> CharacterSetNames =
        new() { "lowercase_letters", "uppercase_letters", "digits" };

    private static readonly List<string> CharacterFilterNames = new();

    private static readonly List<string> TokenFilterNames = new() { "lowercase", "suffix" };

    public PresetAnalyzer GetPresetAnalyzer()
    {
        return new PresetAnalyzer
        {
            Name = "standard",
            Analyzer = new Analyzer
            {
                CharacterFilterNames = CharacterFilterNames,
                CharacterSetNames = CharacterSetNames,
                TokenizerName = TokenizerName,
                TokenFilterNames = TokenFilterNames
            }
        };
    }
}
