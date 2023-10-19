using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Common;
using ProtoBuf;

namespace Persistify.Dtos.Templates.Fields;

[ProtoContract]
public class TextFieldDto : FieldDto
{
    [ProtoMember(3)]
    public AnalyzerDto Analyzer { get; set; } = default!;
}
