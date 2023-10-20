using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Dtos.PresetAnalyzers;

[ProtoContract]
public class FullAnalyzerDto : AnalyzerDto
{
    public FullAnalyzerDto()
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
