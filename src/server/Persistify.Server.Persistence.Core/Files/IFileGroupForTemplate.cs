using System.Collections.Generic;

namespace Persistify.Server.Persistence.Core.Files;

public interface IFileGroupForTemplate
{
    string FileGroupName { get; }
    List<string> GetFileNamesForTemplate(int templateId);
}
