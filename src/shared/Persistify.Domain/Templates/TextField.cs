using System.ComponentModel.DataAnnotations;
using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class TextField
{
    [ProtoMember(1)]
    public string Name { get; set; } = default!;

    [ProtoMember(2)]
    public bool Required { get; set; }

    [ProtoMember(3), Required]
    public AnalyzerDescriptor AnalyzerDescriptor { get; set; } = default!;
}
