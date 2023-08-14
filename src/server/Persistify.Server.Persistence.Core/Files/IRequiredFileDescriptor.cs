using System.Collections.Generic;

namespace Persistify.Server.Persistence.Core.Files;

public interface IRequiredFileDescriptor
{
    List<string> GetRequiredFilesNames();
}
