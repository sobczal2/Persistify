using Microsoft.IO;

namespace Persistify.Server.Serialization.Tests.Unit;

public class ProtobufSerializerTests : SerializerTests<ProtobufSerializer>
{
    protected override ProtobufSerializer CreateSut()
    {
        return new ProtobufSerializer(new RecyclableMemoryStreamManager());
    }
}
