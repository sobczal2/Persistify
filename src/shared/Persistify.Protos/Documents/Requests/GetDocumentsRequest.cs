using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Requests;

[DataContract]
public class GetDocumentsRequest
{
    [DataMember(Order = 1)] public string TemplateName { get; set; } = default!;

    [DataMember(Order = 2)] public long[] DocumentIds { get; set; } = default!;
}
