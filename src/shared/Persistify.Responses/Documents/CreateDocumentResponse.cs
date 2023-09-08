using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class CreateDocumentResponse
{
    public CreateDocumentResponse(int documentId)
    {
        DocumentId = documentId;
    }

    [ProtoMember(1)]
    public int DocumentId { get; set; }
}
