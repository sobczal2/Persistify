using Persistify.Domain.Documents;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class GetDocumentResponse
{
    public GetDocumentResponse(Document document)
    {
        Document = document;
    }

    [ProtoMember(1)]
    public Document Document { get; set; }
}
