using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Server.Domain.Templates;

[ProtoContract]
public class Analyzer
{
    public Analyzer()
    {
        CharacterFilterNames = new List<string>();
        CharacterSetNames = new List<string>();
        TokenFilterNames = new List<string>();
    }

    [ProtoMember(1)]
    public List<string> CharacterFilterNames { get; set; }

    [ProtoMember(2)]
    public List<string> CharacterSetNames { get; set; }

    [ProtoMember(3)]
    public string TokenizerName { get; set; } = default!;

    [ProtoMember(4)]
    public List<string> TokenFilterNames { get; set; }
}
