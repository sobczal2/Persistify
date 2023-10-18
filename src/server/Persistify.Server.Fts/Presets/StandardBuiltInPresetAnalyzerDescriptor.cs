using System.Collections.Generic;
using Persistify.Domain.PresetAnalyzerDescriptors;

namespace Persistify.Server.Fts.Presets;

public class StandardBuiltInPresetAnalyzerDescriptor : IBuiltInPresetAnalyzerDescriptor
{
    private const string TokenizerName = "whitespace";

    private static readonly List<string> CharacterFilterNames =
        new() { "lowercase_letters", "uppercase_letters", "digits" };

    private static readonly List<string> TokenFilterNames = new() { "lowercase", "suffix" };
    public PresetAnalyzerDescriptor GetDescriptor()
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
