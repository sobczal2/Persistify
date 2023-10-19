using ProtoBuf;

namespace Persistify.Server.Domain.Templates;

[ProtoContract]
[ProtoInclude(100, typeof(BoolField))]
[ProtoInclude(101, typeof(NumberField))]
[ProtoInclude(102, typeof(TextField))]
public abstract class Field
{
    [ProtoMember(1)]
    public string Name { get; set; } = default!;

    [ProtoMember(2)]
    public bool Required { get; set; }

    public abstract FieldType FieldType { get; }
}
