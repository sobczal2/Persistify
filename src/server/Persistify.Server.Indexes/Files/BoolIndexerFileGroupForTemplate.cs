using System.Collections.Generic;
using System.IO;
using System.Linq;
using Persistify.Domain.Templates;
using Persistify.Server.Files;

namespace Persistify.Server.Indexes.Files;

public class BoolIndexerFileGroupForTemplate : IFileGroupForTemplate
{
    public string FileGroupName => "BoolIndexer";

    public static string TrueValuesFileName(int templateId, string fieldName)
    {
        return Path.Join("Indexers", "Bool", templateId.ToString("x8"), fieldName, "true-values.bin");
    }

    public static string FalseValuesFileName(int templateId, string fieldName)
    {
        return Path.Join("Indexers", "Bool", templateId.ToString("x8"), fieldName, "false-values.bin");
    }

    public List<string> GetFileNamesForTemplate(Template template)
    {
        var boolFieldNames = template
            .BoolFields
            .Select(field => field.Name)
            .ToList();

        var fileNames = new List<string>();

        foreach (var fieldName in boolFieldNames)
        {
            fileNames.Add(TrueValuesFileName(template.Id, fieldName));
            fileNames.Add(FalseValuesFileName(template.Id, fieldName));
        }

        return fileNames;
    }
}
