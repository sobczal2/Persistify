using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class CreateTemplateResponse
{
    public CreateTemplateResponse(int templateId)
    {
        TemplateId = templateId;
    }

    [ProtoMember(1)] public int TemplateId { get; set; }
}
