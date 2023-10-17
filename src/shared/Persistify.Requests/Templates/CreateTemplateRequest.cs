using System.Collections.Generic;
using Persistify.Domain.Templates;
using Persistify.Requests.Common;
using Persistify.Responses.Templates;
using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class CreateTemplateRequest : IRequest<CreateTemplateResponse>
{
    public CreateTemplateRequest()
    {
        TextFields = new List<TextField>(0);
        NumberFields = new List<NumberField>(0);
        BoolFields = new List<BoolField>(0);
    }

    [ProtoMember(1)]
    public string TemplateName { get; set; } = default!;

    [ProtoMember(2)]
    public List<TextField> TextFields { get; set; } = default!;

    [ProtoMember(3)]
    public List<NumberField> NumberFields { get; set; } = default!;

    [ProtoMember(4)]
    public List<BoolField> BoolFields { get; set; } = default!;
}
