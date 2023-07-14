using System;
using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Requests;

[DataContract]
public class DeleteDocumentsRequest
{
    public DeleteDocumentsRequest()
    {
        TemplateName = null!;
        DocumentIds = Array.Empty<long>();
    }

    [DataMember(Order = 1)] public string TemplateName { get; set; }

    [DataMember(Order = 2)] public long[] DocumentIds { get; set; }
}
