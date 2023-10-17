using Persistify.Domain.Documents;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class GetDocumentResponse : IResponse
{
    [ProtoMember(1)]
    public Document Document { get; set; } = default!;
}
