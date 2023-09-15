using System;
using System.IO;

namespace Persistify.Server.Serialization;

public class JsonSerializer : ISerializer
{
    public ReadOnlyMemory<byte> Serialize<T>(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);
    }

    public T Deserialize<T>(ReadOnlyMemory<byte> bytes)
    {
        if (bytes.IsEmpty)
        {
            throw new InvalidDataException();
        }

        return System.Text.Json.JsonSerializer.Deserialize<T>(bytes.Span) ?? throw new InvalidOperationException();
    }
}
