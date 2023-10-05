using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class GetTemplateRequest
{
    [ProtoMember(1)]
    public string TemplateName { get; set; } = default!;
}
