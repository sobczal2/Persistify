using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class IndexDocumentResponse
{
    [ProtoMember(1)]
    public long DocumentId { get; set; }
}
