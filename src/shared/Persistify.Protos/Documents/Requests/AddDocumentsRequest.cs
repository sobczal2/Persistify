using System;
using System.Runtime.Serialization;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Protos.Documents.Requests;

[DataContract]
public class AddDocumentsRequest
{
    public AddDocumentsRequest()
    {
        TemplateName = null!;
        Documents = Array.Empty<Document>();
    }

    [DataMember(Order = 1)] public string TemplateName { get; set; }

    [DataMember(Order = 2)] public Document[] Documents { get; set; }
}
