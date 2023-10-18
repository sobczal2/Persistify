using System.Collections.Generic;
using System.IO;
using Persistify.Server.Files;

namespace Persistify.Server.Management.Managers.PresetAnalyzerDescriptors;

public class PresetAnalyzerDescriptorManagerRequiredFileGroup : IRequiredFileGroup
{
    public static string IdentifierRepositoryFileName => Path.Join("PresetAnalyzerDescriptor", "identifier.bin");
    public static string TemplateRepositoryMainFileName => Path.Join("PresetAnalyzerDescriptor", "object.bin");

    public static string TemplateRepositoryOffsetLengthFileName =>
        Path.Join("PresetAnalyzerDescriptor", "offsetLength.bin");

    public string FileGroupName => "PresetAnalyzerDescriptorManager";

    public IEnumerable<string> GetFileNames()
    {
        return new List<string>
        {
            IdentifierRepositoryFileName, TemplateRepositoryMainFileName, TemplateRepositoryOffsetLengthFileName
        };
    }
}
