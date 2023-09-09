using ProtoBuf;

namespace Persistify.Requests.Documents;

[ProtoContract]
public class GetDocumentRequest
{
    [ProtoMember(1)]
    public int TemplateId { get; set; }

    [ProtoMember(2)]
    public int DocumentId { get; set; }
}
