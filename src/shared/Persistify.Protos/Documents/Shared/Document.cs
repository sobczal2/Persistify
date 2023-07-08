using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class Document
{
    [DataMember(Order = 1)]
    public TextField[] TextFields { get; set; } = default!;

    [DataMember(Order = 2)]
    public NumberField[] NumberFields { get; set; } = default!;

    [DataMember(Order = 3)]
    public BoolField[] BoolFields { get; set; } = default!;
}
