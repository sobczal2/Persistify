using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Templates;

public class FullAnalyzerDescriptor : AnalyzerDescriptor
{
    public FullAnalyzerDescriptor()
    {
        CharacterFilterNames = new List<string>(0);
        TokenFilterNames = new List<string>(0);
    }

    [ProtoMember(1)]
    public IEnumerable<string> CharacterFilterNames { get; set; }

    [ProtoMember(2)]
    public string TokenizerName { get; set; } = default!;

    [ProtoMember(3)]
    public IEnumerable<string> TokenFilterNames { get; set; }
}
