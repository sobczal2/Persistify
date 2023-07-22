using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Documents;

[ProtoContract]
public class Document
{
    [ProtoMember(1)]
    public long Id { get; set; }

    [ProtoMember(2)]
    public List<TextFieldValue> TextFieldValues { get; set; } = default!;

    [ProtoMember(3)]
    public List<NumberFieldValue> NumberFieldValues { get; set; } = default!;

    [ProtoMember(4)]
    public List<BoolFieldValue> BoolFieldValues { get; set; } = default!;
}
