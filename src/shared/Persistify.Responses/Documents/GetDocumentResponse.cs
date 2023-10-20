using Persistify.Dtos.Documents.Common;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class GetDocumentResponse : IResponse
{
    [ProtoMember(1)]
    public DocumentDto DocumentDto { get; set; } = default!;
}
