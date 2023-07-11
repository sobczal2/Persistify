using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class NumberQuery
{
    [DataMember(Order = 1)] public string FieldName { get; set; } = default!;

    [DataMember(Order = 2)] public double MinValue { get; set; }

    [DataMember(Order = 3)] public double MaxValue { get; set; }
}
