using System.Runtime.Serialization;

namespace Persistify.Protos.Templates.Shared;

[DataContract]
public class Template
{
    [DataMember(Order = 1)]
    public string Name { get; set; } = default!;

    [DataMember(Order = 2)]
    public Field[] Fields { get; set; } = default!;
}
