using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class DocumentWithId
{
    public DocumentWithId()
    {
        Id = default;
        Document = null!;
    }

    [DataMember(Order = 1)] public long Id { get; set; }

    [DataMember(Order = 2)] public Document Document { get; set; }
}
