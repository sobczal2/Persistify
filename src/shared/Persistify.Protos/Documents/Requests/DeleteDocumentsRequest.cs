using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Requests;

[DataContract]
public class DeleteDocumentsRequest
{
    [DataMember(Order = 1)]
    public string TemplateName { get; set; } = default!;

    [DataMember(Order = 2)]
    public ulong[] DocumentIds { get; set; } = default!;
}
