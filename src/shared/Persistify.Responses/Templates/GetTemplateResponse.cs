using Persistify.Dtos.Templates.Common;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class GetTemplateResponse : IResponse
{
    [ProtoMember(1)]
    public TemplateDto TemplateDto { get; set; } = default!;
}
