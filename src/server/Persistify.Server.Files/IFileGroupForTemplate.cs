using System.Collections.Generic;
using Persistify.Server.Domain.Templates;

namespace Persistify.Server.Files;

public interface IFileGroupForTemplate
{
    string FileGroupName { get; }
    IEnumerable<string> GetFileNamesForTemplate(Template template);
}
