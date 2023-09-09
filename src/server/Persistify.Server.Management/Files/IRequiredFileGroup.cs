using System.Collections.Generic;

namespace Persistify.Server.Management.Files;

public interface IRequiredFileGroup
{
    string FileGroupName { get; }
    List<string> GetFileNames();
}
