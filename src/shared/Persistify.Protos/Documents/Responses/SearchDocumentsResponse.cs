using System.Runtime.Serialization;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Protos.Documents.Responses;

[DataContract]
public class SearchDocumentsResponse
{
    public SearchDocumentsResponse(DocumentWithId[] documents, long totalCount)
    {
        Documents = documents;
        TotalCount = totalCount;
    }

    [DataMember(Order = 1)] public DocumentWithId[] Documents { get; set; }

    [DataMember(Order = 2)] public long TotalCount { get; set; }
}
