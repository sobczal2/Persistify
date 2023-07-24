using System.IO;

namespace Persistify.Server.Serialization;

public interface ISerializer
{
    void Serialize<T>(Stream stream, T obj);
    byte[] Serialize<T>(T obj);
    T Deserialize<T>(Stream stream);
    T Deserialize<T>(byte[] bytes);
}
