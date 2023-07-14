using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class NumberField
{
    public NumberField()
    {
        FieldName = null!;
        Value = default;
    }

    [DataMember(Order = 1)] public string FieldName { get; set; }

    [DataMember(Order = 2)] public double Value { get; set; }
}
