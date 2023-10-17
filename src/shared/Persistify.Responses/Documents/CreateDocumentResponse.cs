using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class CreateDocumentResponse : IResponse
{
    [ProtoMember(1)]
    public int DocumentId { get; set; }
}
