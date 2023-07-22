using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class NumberField
{
    [ProtoMember(1)]
    public string Name { get; set; } = default!;

    [ProtoMember(2)]
    public bool IsRequired { get; set; }
}
