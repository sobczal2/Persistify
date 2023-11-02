using Persistify.Dtos.PresetAnalyzers;
using ProtoBuf;

namespace Persistify.Dtos.Templates.Fields;

[ProtoContract]
public class TextFieldDto : FieldDto
{
    [ProtoMember(3)]
    public bool IndexText { get; set; }

    [ProtoMember(4)]
    public bool IndexFullText { get; set; }

    [ProtoMember(5)]
    public AnalyzerDto? AnalyzerDto { get; set; }
}
