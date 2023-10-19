using ProtoBuf;

namespace Persistify.Server.Domain.Templates;

[ProtoContract]
public class NumberField : Field
{
    public override FieldType FieldType => FieldType.Number;
}
