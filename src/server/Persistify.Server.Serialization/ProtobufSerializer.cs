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

    static ProtobufSerializer()
    {
        Serializer.PrepareSerializer<Template>();
        Serializer.PrepareSerializer<Document>();
    }

    public ProtobufSerializer(RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        _recyclableMemoryStreamManager = recyclableMemoryStreamManager;
    }
    public ReadOnlyMemory<byte> Serialize<T>(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        using var stream = _recyclableMemoryStreamManager.GetStream();
        Serializer.Serialize(stream, obj);
        return new ReadOnlyMemory<byte>(stream.GetBuffer(), 0, (int)stream.Length);
    }

    public T Deserialize<T>(ReadOnlyMemory<byte> bytes)
    {
        if (bytes.IsEmpty)
        {
            throw new InvalidDataException();
        }

        return Serializer.Deserialize<T>(bytes);
    }
}
