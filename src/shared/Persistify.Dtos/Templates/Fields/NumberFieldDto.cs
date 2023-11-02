using ProtoBuf;

namespace Persistify.Dtos.Templates.Fields;

[ProtoContract]
public class NumberFieldDto : FieldDto
{
    [ProtoMember(3)]
    public bool Index { get; set; }
}
