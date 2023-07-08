using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class BoolQuery
{
    [DataMember(Order = 1)]
    public string FieldName { get; set; } = default!;

    [DataMember(Order = 2)]
    public bool Value { get; set; }
}
