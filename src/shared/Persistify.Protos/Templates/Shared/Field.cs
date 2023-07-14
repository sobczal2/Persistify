using System.Runtime.Serialization;

namespace Persistify.Protos.Templates.Shared;

[DataContract]
public class Field
{
    public Field()
    {
        Name = null!;
        Type = default!;
        IsRequired = default!;
    }

    [DataMember(Order = 1)] public string Name { get; set; }

    [DataMember(Order = 2)] public FieldType Type { get; set; }

    [DataMember(Order = 3)] public bool IsRequired { get; set; }
}
