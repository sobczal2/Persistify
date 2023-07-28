using System.Collections.Generic;
using Persistify.Domain.Documents;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class SearchDocumentsResponse
{
    [ProtoMember(1)]
    public List<Document> Documents { get; set; }

    [ProtoMember(2)]
    public long TotalCount { get; set; }

    public SearchDocumentsResponse(List<Document> documents, long totalCount)
    {
        Documents = documents;
        TotalCount = totalCount;
    }
}
