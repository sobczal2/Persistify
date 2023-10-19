using System.Collections.Generic;
using Persistify.Server.Domain.PresetAnalyzers;

namespace Persistify.Server.Fts.PresetAnalyzers;

public class StandardBuiltInPresetAnalyzer : IBuiltInPresetAnalyzer
{
    private const string TokenizerName = "whitespace";

    private static readonly List<string> CharacterFilterNames =
        new() { "lowercase_letters", "uppercase_letters", "digits" };

    private static readonly List<string> TokenFilterNames = new() { "lowercase", "suffix" };
    public PresetAnalyzer GetPresetAnalyzer()
    {
        return new()
        {
            Name = "standard",
            Analyzer = new()
            {
                TokenizerName = TokenizerName,
                CharacterFilterNames = CharacterFilterNames,
                TokenFilterNames = TokenFilterNames
            }
        };
    }
}
