using Persistify.Domain.Documents;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class GetDocumentResponse
{
    [ProtoMember(1)]
    public Document Document { get; set; }

    public GetDocumentResponse(Document document)
    {
        Document = document;
    }
}
