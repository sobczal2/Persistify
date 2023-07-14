using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class BoolQuery
{
    public BoolQuery()
    {
        FieldName = null!;
        Value = default;
    }

    [DataMember(Order = 1)] public string FieldName { get; set; }

    [DataMember(Order = 2)] public bool Value { get; set; }
}
