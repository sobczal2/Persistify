using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class CreateTemplateResponse
{
    [ProtoMember(1)]
    public long TemplateId { get; set; }
}
