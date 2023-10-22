using System.Collections.Generic;
using System.IO;
using Persistify.Server.Domain.Templates;
using Persistify.Server.Files;

namespace Persistify.Server.Management.Managers.Documents;

public class DocumentManagerFileGroupForTemplate : IFileGroupForTemplate
{
    public string FileGroupName => "DocumentRepository";

    public IEnumerable<string> GetFileNamesForTemplate(Template template)
    {
        return new List<string>
        {
            IdentifierFileName(template.Id),
            DocumentRepositoryMainFileName(template.Id),
            DocumentRepositoryOffsetLengthFileName(template.Id)
        };
    }

    public static string IdentifierFileName(int templateId)
    {
        return Path.Join("Document", templateId.ToString("x8"), "identifier.bin");
    }

    public static string DocumentRepositoryMainFileName(int templateId)
    {
        return Path.Join("Document", templateId.ToString("x8"), "object.bin");
    }

    public static string DocumentRepositoryOffsetLengthFileName(int templateId)
    {
        return Path.Join("Document", templateId.ToString("x8"), "offsetLength.bin");
    }
}
