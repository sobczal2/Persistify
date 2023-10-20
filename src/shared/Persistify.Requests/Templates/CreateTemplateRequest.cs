using System.Collections.Generic;
using Persistify.Dtos.Templates;
using Persistify.Dtos.Templates.Fields;
using Persistify.Requests.Common;
using Persistify.Responses.Templates;
using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class CreateTemplateRequest : IRequest<CreateTemplateResponse>
{
    [ProtoMember(1)]
    public string TemplateName { get; set; } = default!;

    [ProtoMember(2)]
    public List<FieldDto> Fields { get; set; }

    public CreateTemplateRequest()
    {
        Fields = new List<FieldDto>();
    }
}
