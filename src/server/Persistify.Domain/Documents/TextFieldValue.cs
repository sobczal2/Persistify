using ProtoBuf;

namespace Persistify.Domain.Documents;

[ProtoContract]
public class TextFieldValue
{
    [ProtoMember(1)]
    public string Value { get; set; } = default!;

    [ProtoMember(2)]
    public string FieldName { get; set; } = default!;
}
