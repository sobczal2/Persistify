using ProtoBuf;

namespace Persistify.Server.Domain.Templates;

[ProtoContract]
public class DateTimeField : Field
{
    public override FieldType FieldType => FieldType.DateTime;
}
