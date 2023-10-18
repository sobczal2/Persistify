using Persistify.Domain.Templates;
using ProtoBuf;

namespace Persistify.Domain.PresetAnalyzerDescriptors;

[ProtoContract]
public class PresetAnalyzerDescriptor
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; } = default!;

    [ProtoMember(3)]
    public AnalyzerDescriptor Analyzer { get; set; } = default!;
}
