using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class Analyzer
{
    [ProtoMember(1)]
    public List<string> CharacterFilterNames { get; set; } = default!;

    [ProtoMember(2)]
    public string TokenizerName { get; set; } = default!;

    [ProtoMember(3)]
    public List<string> TokenFilterNames { get; set; } = default!;
}
