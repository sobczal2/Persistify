using ProtoBuf;

namespace Persistify.Domain.Documents;

[ProtoContract]
public class BoolFieldValue : FieldValue
{
    [ProtoMember(2)]
    public bool Value { get; set; }
}
