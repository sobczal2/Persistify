using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class TextField : Field
{
    public override FieldType FieldType => FieldType.Text;

    [ProtoMember(3)]
    public Analyzer Analyzer { get; set; } = default!;
}
