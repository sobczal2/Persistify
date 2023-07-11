using System.Runtime.Serialization;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Protos.Documents.Responses;

[DataContract]
public class SearchDocumentsResponse
{
    [DataMember(Order = 1)] public DocumentWithId[] Documents { get; set; } = default!;

    [DataMember(Order = 2)] public long TotalCount { get; set; }
}
