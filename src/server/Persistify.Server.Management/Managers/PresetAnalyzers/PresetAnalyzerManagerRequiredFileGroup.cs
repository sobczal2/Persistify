using System.Collections.Generic;
using System.IO;
using Persistify.Server.Files;

namespace Persistify.Server.Management.Managers.PresetAnalyzers;

public class PresetAnalyzerManagerRequiredFileGroup : IRequiredFileGroup
{
    public static string IdentifierRepositoryFileName =>
        Path.Join("PresetAnalyzer", "identifier.bin");

    public static string PresetAnalyzerRepositoryMainFileName =>
        Path.Join("PresetAnalyzer", "object.bin");

    public static string PresetAnalyzerRepositoryOffsetLengthFileName =>
        Path.Join("PresetAnalyzer", "offsetLength.bin");

    public string FileGroupName => "PresetAnalyzerDescriptorManager";

    public IEnumerable<string> GetFileNames()
    {
        return new List<string>
        {
            IdentifierRepositoryFileName,
            PresetAnalyzerRepositoryMainFileName,
            PresetAnalyzerRepositoryOffsetLengthFileName
        };
    }
}
