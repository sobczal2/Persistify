﻿using System.Collections.Generic;
using System.IO;
using Persistify.Server.Management.Files;

namespace Persistify.Server.Management.Managers.Documents;

public class DocumentManagerFileGroupForTemplate : IFileGroupForTemplate
{
    public string FileGroupName => "DocumentRepository";

    public List<string> GetFileNamesForTemplate(int templateId)
    {
        return new List<string>
        {
            IdentifierFileName(templateId),
            DocumentRepositoryMainFileName(templateId),
            DocumentRepositoryOffsetLengthFileName(templateId)
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