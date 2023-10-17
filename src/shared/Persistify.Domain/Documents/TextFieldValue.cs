using ProtoBuf;

namespace Persistify.Domain.Documents;

[ProtoContract]
public class TextFieldValue : FieldValue
{
    [ProtoMember(2)]
    public string Value { get; set; } = default!;
}
