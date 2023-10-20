using System.Collections.Generic;
using Persistify.Dtos.Templates.Fields;
using ProtoBuf;

namespace Persistify.Dtos.Templates.Common;

[ProtoContract]
public class TemplateDto
{
    [ProtoMember(1)]
    public string Name { get; set; } = default!;

    [ProtoMember(2)]
    public List<FieldDto> Fields { get; set; }

    public TemplateDto()
    {
        Fields = new List<FieldDto>();
    }
}
