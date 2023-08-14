using System.IO;

namespace Persistify.Server.Persistence.Core.Files;

public interface IFileStreamFactory
{
    Stream CreateStream(string relativePath);
}
