using ProtoBuf;

namespace Persistify.Dtos.Templates.Fields;

[ProtoContract]
[ProtoInclude(100, typeof(BoolFieldDto))]
[ProtoInclude(101, typeof(NumberFieldDto))]
[ProtoInclude(102, typeof(TextFieldDto))]
[ProtoInclude(103, typeof(DateTimeFieldDto))]
[ProtoInclude(104, typeof(BinaryFieldDto))]
public class FieldDto
{
    [ProtoMember(1)]
    public string Name { get; set; } = default!;

    [ProtoMember(2)]
    public bool Required { get; set; }
}
