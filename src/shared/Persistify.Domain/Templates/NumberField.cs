using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class NumberField : Field
{
    public override FieldType FieldType => FieldType.Number;
}
