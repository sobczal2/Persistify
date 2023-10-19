using ProtoBuf;

namespace Persistify.Dtos.PresetAnalyzers;

[ProtoContract]
public class PresetAnalyzerDto
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string PresetAnalyzerName { get; set; } = default!;

    [ProtoMember(3)]
    public FullAnalyzerDto FullAnalyzerDto { get; set; } = default!;
}
