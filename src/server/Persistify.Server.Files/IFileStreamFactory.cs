using System.IO;

namespace Persistify.Server.Files;

public interface IFileStreamFactory
{
    Stream CreateStream(string relativePath);
}
