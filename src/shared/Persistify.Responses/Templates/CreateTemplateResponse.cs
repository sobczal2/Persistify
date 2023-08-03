using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class CreateTemplateResponse
{
    [ProtoMember(1)]
    public int TemplateId { get; set; }

    public CreateTemplateResponse(int templateId)
    {
        TemplateId = templateId;
    }
}
