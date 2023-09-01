using System.IO;

namespace Persistify.Server.Management.Files;

public interface IFileStreamFactory
{
    Stream CreateStream(string relativePath);
}
