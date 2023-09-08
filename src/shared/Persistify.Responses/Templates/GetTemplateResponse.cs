using Persistify.Domain.Templates;
using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class GetTemplateResponse
{
    public GetTemplateResponse(Template template)
    {
        Template = template;
    }

    [ProtoMember(1)]
    public Template Template { get; set; }
}
