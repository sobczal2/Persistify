using ProtoBuf;

namespace Persistify.Dtos.PresetAnalyzers;

[ProtoContract]
public class PresetNameAnalyzerDto : AnalyzerDto
{
    [ProtoMember(1)]
    public string PresetName { get; set; } = default!;
}
