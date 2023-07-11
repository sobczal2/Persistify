using System.Runtime.Serialization;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Protos.Documents.Requests;

[DataContract]
public class AddDocumentsRequest
{
    [DataMember(Order = 1)] public string TemplateName { get; set; } = default!;

    [DataMember(Order = 2)] public Document[] Documents { get; set; } = default!;
}
