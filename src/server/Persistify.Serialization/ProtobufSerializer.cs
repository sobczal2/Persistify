using System;
using System.Buffers;
using System.IO;
using Persistify.Protos.Documents.Shared;
using Persistify.Protos.Templates.Shared;
using ProtoBuf;

namespace Persistify.Serialization;

public class ProtobufSerializer : ISerializer
{
    public ProtobufSerializer()
    {
        Serializer.PrepareSerializer<Template>();
        Serializer.PrepareSerializer<Document>();
    }
    public void Serialize<T>(Stream stream, T obj)
    {
        Serializer.Serialize(stream, obj);
    }

    public T Deserialize<T>(Stream stream)
    {
        return Serializer.Deserialize<T>(stream);
    }
}
