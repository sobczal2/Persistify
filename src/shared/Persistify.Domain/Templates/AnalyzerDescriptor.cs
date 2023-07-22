using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class AnalyzerDescriptor
{
    public AnalyzerDescriptor()
    {
        CharacterFilterNames = new List<string>(0);
        TokenFilterNames = new List<string>(0);
    }

    [ProtoMember(1)]
    public List<string> CharacterFilterNames { get; set; }

    [ProtoMember(2)]
    public string TokenizerName { get; set; } = default!;

    [ProtoMember(3)]
    public List<string> TokenFilterNames { get; set; }
}
