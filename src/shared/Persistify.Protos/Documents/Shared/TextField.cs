using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class TextField
{
    public TextField()
    {
        FieldName = null!;
        Value = null!;
    }

    [DataMember(Order = 1)] public string FieldName { get; set; }

    [DataMember(Order = 2)] public string Value { get; set; }
}
