using System.Collections.Generic;
using System.IO;
using Persistify.Server.Management.Files;

namespace Persistify.Server.Management.Managers.Templates;

public class TemplateManagerRequiredFileGroup : IRequiredFileGroup
{
    public string FileGroupName => "TemplateManager";

    public List<string> GetFileNames() => new()
    {
        IdentifierRepositoryFileName,
        TemplateRepositoryMainFileName,
        TemplateRepositoryOffsetLengthFileName
    };

    public static string IdentifierRepositoryFileName => Path.Join("Template", "identifier.bin");
    public static string TemplateRepositoryMainFileName => Path.Join("Template", "object.bin");
    public static string TemplateRepositoryOffsetLengthFileName => Path.Join("Template", "offsetLength.bin");

}
