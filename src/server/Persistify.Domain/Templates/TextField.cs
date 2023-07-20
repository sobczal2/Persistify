using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class TextField
{
    [ProtoMember(1)] public string Name { get; set; } = default!;
    [ProtoMember(2)] public bool IsRequired { get; set; }
    [ProtoMember(3)] public string? AnalyzerPresetName { get; set; }
    [ProtoMember(4)] public AnalyzerDescriptor? AnalyzerDescriptor { get; set; }
}
