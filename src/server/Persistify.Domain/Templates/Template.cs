using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class Template
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; } = default!;

    [ProtoMember(3)]
    public List<TextField> TextFields { get; set; } = default!;

    [ProtoMember(4)]
    public List<NumberField> NumberFields { get; set; } = default!;

    [ProtoMember(5)]
    public List<BoolField> BoolFields { get; set; } = default!;
}
