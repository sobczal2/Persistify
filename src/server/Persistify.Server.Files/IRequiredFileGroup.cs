using System.Collections.Generic;

namespace Persistify.Server.Files;

public interface IRequiredFileGroup
{
    string FileGroupName { get; }
    List<string> GetFileNames();
}
