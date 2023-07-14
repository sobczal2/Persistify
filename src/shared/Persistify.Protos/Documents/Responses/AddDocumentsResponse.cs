using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Responses;

[DataContract]
public class AddDocumentsResponse
{
    public AddDocumentsResponse(long[] documentIds)
    {
        DocumentIds = documentIds;
    }

    [DataMember(Order = 1)] public long[] DocumentIds { get; set; }
}
