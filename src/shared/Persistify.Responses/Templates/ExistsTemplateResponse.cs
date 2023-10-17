using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class ExistsTemplateResponse
{
    [ProtoMember(1)]
    public bool Exists { get; set; }
}
