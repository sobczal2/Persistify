using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class FtsQuery
{
    [DataMember(Order = 1)] public string FieldName { get; set; } = default!;

    [DataMember(Order = 2)] public string Value { get; set; } = default!;

    [DataMember(Order = 3)] public bool ExactMatch { get; set; }

    [DataMember(Order = 4)] public bool CaseSensitive { get; set; }
}
