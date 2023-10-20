using Persistify.Dtos.PresetAnalyzers;
using ProtoBuf;

namespace Persistify.Dtos.Templates.Fields;

[ProtoContract]
public class TextFieldDto : FieldDto
{
    [ProtoMember(3)]
    public AnalyzerDto AnalyzerDto { get; set; } = default!;
}
