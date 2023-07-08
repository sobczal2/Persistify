using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Responses;

[DataContract]
public class AddDocumentsResponse
{
    [DataMember(Order = 1)]
    public ulong[] DocumentIds { get; set; } = default!;
}
