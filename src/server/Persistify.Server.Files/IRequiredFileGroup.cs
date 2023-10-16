using System.Collections.Generic;

namespace Persistify.Server.Files;

public interface IRequiredFileGroup
{
    string FileGroupName { get; }
    IEnumerable<string> GetFileNames();
}
