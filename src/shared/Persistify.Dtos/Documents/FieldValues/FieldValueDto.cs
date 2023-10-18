using Persistify.Domain.Templates;
using ProtoBuf;

namespace Persistify.Dtos.Documents.FieldValues;

[ProtoContract]
[ProtoInclude(100, typeof(BoolFieldValueDto))]
[ProtoInclude(101, typeof(NumberFieldValueDto))]
[ProtoInclude(102, typeof(TextFieldValueDto))]
public abstract class FieldValueDto
{
    [ProtoMember(1)]
    public string FieldName { get; set; } = default!;

    public abstract FieldType FieldType { get; }
}
