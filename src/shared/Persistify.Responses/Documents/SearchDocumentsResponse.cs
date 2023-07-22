using System.Collections.Generic;
using Persistify.Domain.Documents;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class SearchDocumentsResponse
{
    [ProtoMember(1)]
    public List<Document> Documents { get; set; } = default!;

    [ProtoMember(2)]
    public long TotalCount { get; set; }
}
