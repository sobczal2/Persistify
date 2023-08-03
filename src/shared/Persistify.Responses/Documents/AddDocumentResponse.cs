using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class AddDocumentResponse
{
    [ProtoMember(1)]
    public int DocumentId { get; set; }

    public AddDocumentResponse(int documentId)
    {
        DocumentId = documentId;
    }
}
