using Persistify.Requests.Common;
using Persistify.Responses.Templates;
using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class DeleteTemplateRequest : IRequest<DeleteTemplateResponse>
{
    [ProtoMember(1)]
    public string TemplateName { get; set; } = default!;
}
