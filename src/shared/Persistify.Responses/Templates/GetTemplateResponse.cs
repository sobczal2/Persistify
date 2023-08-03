using Persistify.Domain.Templates;
using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class GetTemplateResponse
{
    [ProtoMember(1)]
    public Template Template { get; set; }

    public GetTemplateResponse(Template template)
    {
        Template = template;
    }
}
