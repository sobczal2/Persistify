using System.Runtime.Serialization;

namespace Persistify.Protos.Templates.Shared;

[DataContract]
public class Field
{
    [DataMember(Order = 1)] public string Name { get; set; } = default!;

    [DataMember(Order = 2)] public FieldType Type { get; set; }

    [DataMember(Order = 3)] public bool IsRequired { get; set; }
}
