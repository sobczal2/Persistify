using System.Collections.Generic;

namespace Persistify.Server.Persistence.Core.Files;

public interface IFileForTemplateDescriptor
{
    List<string> GetFilesNamesForTemplate(int templateId);
}
