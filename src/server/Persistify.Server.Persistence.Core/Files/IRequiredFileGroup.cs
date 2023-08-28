using System.Collections.Generic;

namespace Persistify.Server.Persistence.Core.Files;

public interface IRequiredFileGroup
{
    string FileGroupName { get; }
    List<string> GetFileNames();
}
