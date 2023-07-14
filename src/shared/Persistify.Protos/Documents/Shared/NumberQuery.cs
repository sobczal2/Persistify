using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class NumberQuery
{
    public NumberQuery()
    {
        FieldName = null!;
        MinValue = default;
        MaxValue = default;
    }

    [DataMember(Order = 1)] public string FieldName { get; set; }

    [DataMember(Order = 2)] public double MinValue { get; set; }

    [DataMember(Order = 3)] public double MaxValue { get; set; }
}
