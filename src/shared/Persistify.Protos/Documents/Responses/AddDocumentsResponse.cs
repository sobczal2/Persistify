using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Responses;

[DataContract]
public class AddDocumentsResponse
{
    [DataMember(Order = 1)]
    public long[] DocumentIds { get; set; } = default!;
}
