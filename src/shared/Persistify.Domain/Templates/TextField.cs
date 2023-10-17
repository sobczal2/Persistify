using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class TextField : Field
{
    [ProtoMember(3)]
    public AnalyzerDescriptor AnalyzerDescriptor { get; set; } = default!;
}
