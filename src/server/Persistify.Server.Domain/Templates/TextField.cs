using ProtoBuf;

namespace Persistify.Server.Domain.Templates;

[ProtoContract]
public class TextField : Field
{
    public override FieldType FieldType => FieldType.Text;

    [ProtoMember(3)]
    public Analyzer Analyzer { get; set; } = default!;
}
