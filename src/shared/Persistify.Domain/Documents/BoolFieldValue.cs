using ProtoBuf;

namespace Persistify.Domain.Documents;

[ProtoContract]
public class BoolFieldValue
{
    [ProtoMember(1)] public bool Value { get; set; }

    [ProtoMember(2)] public string FieldName { get; set; } = default!;
}
