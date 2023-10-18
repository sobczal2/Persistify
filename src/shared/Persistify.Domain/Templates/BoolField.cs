using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class BoolField : Field
{
    public override FieldType FieldType => FieldType.Bool;
}
