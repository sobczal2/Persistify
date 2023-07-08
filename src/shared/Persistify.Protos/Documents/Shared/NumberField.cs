using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class NumberField
{
    [DataMember(Order = 1)]
    public string FieldName { get; set; } = default!;

    [DataMember(Order = 2)]
    public double Value { get; set; }
}
