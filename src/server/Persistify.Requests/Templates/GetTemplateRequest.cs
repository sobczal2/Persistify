using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class GetTemplateRequest
{
    [ProtoMember(1)] public int TemplateId { get; set; }
}
