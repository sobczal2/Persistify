using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class PresetAnalyzerDescriptor : AnalyzerDescriptor
{
    [ProtoMember(1)]
    public string PresetName { get; set; } = default!;
}
