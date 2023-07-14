using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class FtsQuery
{
    public FtsQuery()
    {
        FieldName = null!;
        Value = null!;
        ExactMatch = default;
        CaseSensitive = default;
    }

    [DataMember(Order = 1)] public string FieldName { get; set; }

    [DataMember(Order = 2)] public string Value { get; set; }

    [DataMember(Order = 3)] public bool ExactMatch { get; set; }

    [DataMember(Order = 4)] public bool CaseSensitive { get; set; }
}
