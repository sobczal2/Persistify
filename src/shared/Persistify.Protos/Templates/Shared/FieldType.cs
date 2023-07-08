using System.Runtime.Serialization;

namespace Persistify.Protos.Templates.Shared;

[DataContract]
public enum FieldType
{
    [EnumMember]
    Text = 0,

    [EnumMember]
    Number = 1,

    [EnumMember]
    Bool = 2,
}
