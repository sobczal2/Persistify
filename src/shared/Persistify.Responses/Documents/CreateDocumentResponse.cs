using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class CreateDocumentResponse
{
    [ProtoMember(1)]
    public int DocumentId { get; set; }

    public CreateDocumentResponse(int documentId)
    {
        DocumentId = documentId;
    }
}
