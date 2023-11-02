using ProtoBuf;

namespace Persistify.Server.Domain.Templates;

[ProtoContract]
public class TextField : Field
{
    public override FieldType FieldType => FieldType.Text;

    [ProtoMember(3)]
    public bool IndexText { get; set; }

    [ProtoMember(4)]
    public bool IndexFullText { get; set; }

    [ProtoMember(5)]
    public Analyzer? Analyzer { get; set; }
}
