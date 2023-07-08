using System.Buffers;
using System.IO;

namespace Persistify.Serialization;

public interface ISerializer
{
    void Serialize<T>(Stream stream, T obj);
    T Deserialize<T>(Stream stream);
}
