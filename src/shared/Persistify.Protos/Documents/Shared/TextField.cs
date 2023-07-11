using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class TextField
{
    [DataMember(Order = 1)] public string FieldName { get; set; } = default!;

    [DataMember(Order = 2)] public string Value { get; set; } = default!;
}
