using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class BoolField
{
    [ProtoMember(1)] public string Name { get; set; } = default!;
    [ProtoMember(2)] public bool IsRequired { get; set; }
}
