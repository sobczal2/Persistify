using System.Runtime.Serialization;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Protos.Documents.Responses;

[DataContract]
public class GetDocumentsResponse
{
    [DataMember(Order = 1)] public DocumentWithId[] Documents { get; set; } = default!;
}
