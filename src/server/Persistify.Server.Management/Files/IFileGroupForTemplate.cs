using System.Collections.Generic;

namespace Persistify.Server.Management.Files;

public interface IFileGroupForTemplate
{
    string FileGroupName { get; }
    List<string> GetFileNamesForTemplate(int templateId);
}
