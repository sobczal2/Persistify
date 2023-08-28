using System.Collections.Generic;
using System.IO;
using Persistify.Server.Persistence.Core.Files;

namespace Persistify.Server.Persistence.Core.Templates;

public class TemplateRepositoryRequiredFileGroup : IRequiredFileGroup
{
    public string FileGroupName => "TemplateRepository";

    public List<string> GetFileNames() => new()
    {
        IdentifierFileName,
        InnerTemplateMainFileName,
        InnerTemplateOffsetLengthFileName
    };

    public string IdentifierFileName => Path.Join("Template", "identifier.bin");
    public string InnerTemplateMainFileName => Path.Join("Template", "object.bin");
    public string InnerTemplateOffsetLengthFileName => Path.Join("Template", "offsetLength.bin");

}
