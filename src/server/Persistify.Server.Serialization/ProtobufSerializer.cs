using System;
using System.IO;
using Microsoft.IO;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using ProtoBuf;

namespace Persistify.Server.Serialization;

public class ProtobufSerializer : ISerializer
{
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

    public ProtobufSerializer(RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        _recyclableMemoryStreamManager = recyclableMemoryStreamManager;
        Serializer.PrepareSerializer<Template>();
        Serializer.PrepareSerializer<Document>();
    }

    public void Serialize<T>(Stream stream, T obj)
    {
        Serializer.Serialize(stream, obj);
    }

    public byte[] Serialize<T>(T obj)
    {
        using var stream = _recyclableMemoryStreamManager.GetStream();
        Serializer.Serialize(stream, obj);
        return stream.ToArray();
    }

    public T Deserialize<T>(Stream stream)
    {
        return Serializer.Deserialize<T>(stream);
    }

    public T Deserialize<T>(byte[] bytes)
    {
        using var stream = _recyclableMemoryStreamManager.GetStream(bytes);
        return Serializer.Deserialize<T>(stream);
    }

    public T Deserialize<T>(ReadOnlyMemory<byte> bytes)
    {
        return Serializer.Deserialize<T>(bytes);
    }
}
