using ProtoBuf;

namespace Persistify.Domain.Documents;

[ProtoContract]
public class NumberFieldValue
{
    [ProtoMember(1)] public double Value { get; set; }

    [ProtoMember(2)] public string FieldName { get; set; } = default!;
}
