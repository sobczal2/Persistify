using Persistify.Domain.Templates;
using Persistify.Dtos.Templates.Common;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class GetTemplateResponse : IResponse
{
    [ProtoMember(1)]
    public TemplateDto Template { get; set; } = default!;
}
