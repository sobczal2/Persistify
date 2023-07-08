using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class DocumentWithId
{
    [DataMember(Order = 1)]
    public ulong Id { get; set; }

    [DataMember(Order = 2)]
    public Document Document { get; set; } = default!;
}
