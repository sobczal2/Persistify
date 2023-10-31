using System.Collections.Generic;
using System.IO;
using Persistify.Server.Files;

namespace Persistify.Server.Management.Managers.Templates;

public class TemplateManagerRequiredFileGroup : IRequiredFileGroup
{
    public static string IdentifierRepositoryFileName => Path.Join("Template", "identifier.bin");
    public static string TemplateRepositoryMainFileName => Path.Join("Template", "object.bin");

    public static string TemplateRepositoryOffsetLengthFileName =>
        Path.Join("Template", "offsetLength.bin");

    public string FileGroupName => "TemplateManager";

    public IEnumerable<string> GetFileNames()
    {
        return new List<string>
        {
            IdentifierRepositoryFileName, TemplateRepositoryMainFileName, TemplateRepositoryOffsetLengthFileName
        };
    }
}
