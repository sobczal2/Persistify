using ProtoBuf;

namespace Persistify.Dtos.PresetAnalyzers;

[ProtoContract]
public class PresetAnalyzerDto
{
    [ProtoMember(1)]
    public string Name { get; set; } = default!;

    [ProtoMember(2)]
    public FullAnalyzerDto FullAnalyzerDto { get; set; } = default!;
}
