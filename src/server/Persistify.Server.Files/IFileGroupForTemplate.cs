using System.Collections.Generic;
using Persistify.Domain.Templates;

namespace Persistify.Server.Files;

public interface IFileGroupForTemplate
{
    string FileGroupName { get; }
    List<string> GetFileNamesForTemplate(Template template);
}
