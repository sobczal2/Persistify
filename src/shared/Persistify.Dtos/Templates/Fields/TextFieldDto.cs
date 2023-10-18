using Persistify.Dtos.Templates.Common;
using ProtoBuf;

namespace Persistify.Dtos.Templates.Fields;

[ProtoContract]
public class TextFieldDto : FieldDto
{
    [ProtoMember(3)]
    public AnalyzerDescriptorDto AnalyzerDescriptor { get; set; } = default!;
}
