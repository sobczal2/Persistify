using ProtoBuf;

namespace Persistify.Server.Domain.Documents;

[ProtoContract]
public class BinaryFieldValue : FieldValue
{
    [ProtoMember(2)]
    public byte[] Value { get; set; } = default!;
}
