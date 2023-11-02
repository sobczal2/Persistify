using ProtoBuf;

namespace Persistify.Server.Domain.Templates;

[ProtoContract]
public class DateTimeField : Field
{
    public override FieldType FieldType => FieldType.DateTime;

    [ProtoMember(3)]
    public bool Index { get; set; }
}
