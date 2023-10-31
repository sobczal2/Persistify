using ProtoBuf;

namespace Persistify.Server.Domain.Documents;

[ProtoContract]
[ProtoInclude(100, typeof(BoolFieldValue))]
[ProtoInclude(101, typeof(NumberFieldValue))]
[ProtoInclude(102, typeof(TextFieldValue))]
[ProtoInclude(103, typeof(DateTimeFieldValue))]
public class FieldValue
{
    [ProtoMember(1)]
    public string FieldName { get; set; } = default!;
}
