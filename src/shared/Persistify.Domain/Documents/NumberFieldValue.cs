using ProtoBuf;

namespace Persistify.Domain.Documents;

[ProtoContract]
public class NumberFieldValue : FieldValue
{
    [ProtoMember(2)]
    public double Value { get; set; }
}
