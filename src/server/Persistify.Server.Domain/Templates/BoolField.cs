using ProtoBuf;

namespace Persistify.Server.Domain.Templates;

[ProtoContract]
public class BoolField : Field
{
    public override FieldType FieldType => FieldType.Bool;

    [ProtoMember(3)]
    public bool Index { get; set; }
}
