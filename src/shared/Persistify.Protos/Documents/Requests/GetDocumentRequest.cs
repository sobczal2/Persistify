using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Requests;

[DataContract]
public class GetDocumentRequest
{
    public GetDocumentRequest()
    {
        TemplateName = null!;
        DocumentId = default;
    }

    [DataMember(Order = 1)] public string TemplateName { get; set; }

    [DataMember(Order = 2)] public long DocumentId { get; set; }
}
