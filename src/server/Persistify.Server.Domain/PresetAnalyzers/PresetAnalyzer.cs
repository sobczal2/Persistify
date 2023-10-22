using Persistify.Server.Domain.Templates;
using ProtoBuf;

namespace Persistify.Server.Domain.PresetAnalyzers;

[ProtoContract]
public class PresetAnalyzer
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; } = default!;

    [ProtoMember(3)]
    public Analyzer Analyzer { get; set; } = default!;
}
