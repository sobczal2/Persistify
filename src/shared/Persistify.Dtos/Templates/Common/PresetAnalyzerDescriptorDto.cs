using ProtoBuf;

namespace Persistify.Dtos.Templates.Common;

[ProtoContract]
public class PresetAnalyzerDescriptorDto : AnalyzerDescriptorDto
{
    [ProtoMember(1)]
    public string PresetName { get; set; } = default!;
}
