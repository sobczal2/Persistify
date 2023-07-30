using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace Persistify.Server.Serialization;

public class JsonSerializer : ISerializer
{
    public void Serialize<T>(Stream stream, T obj)
    {
        throw new NotImplementedException();
    }

    public ReadOnlyMemory<byte> Serialize<T>(T obj)
    {
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);
    }

    public T Deserialize<T>(Stream stream)
    {
        throw new NotImplementedException();
    }

    public T Deserialize<T>(byte[] bytes)
    {
        throw new NotImplementedException();
    }

    public T Deserialize<T>(ReadOnlyMemory<byte> bytes)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(bytes.Span) ?? throw new InvalidOperationException();
    }
}
