using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
[ProtoInclude(100, typeof(BoolField))]
[ProtoInclude(101, typeof(NumberField))]
[ProtoInclude(102, typeof(TextField))]
public class Field
{
    [ProtoMember(1)]
    public string Name { get; set; } = default!;

    [ProtoMember(2)]
    public bool Required { get; set; }
}
