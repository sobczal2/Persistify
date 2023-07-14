using System.Runtime.Serialization;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Protos.Documents.Responses;

[DataContract]
public class GetDocumentResponse
{
    public GetDocumentResponse(DocumentWithId document)
    {
        Document = document;
    }

    [DataMember(Order = 1)] public DocumentWithId Document { get; set; }
}
