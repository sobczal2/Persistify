using Persistify.Domain.Templates;
using ProtoBuf;

namespace Persistify.Dtos.Documents.FieldValues;

[ProtoContract]
public class TextFieldValueDto : FieldValueDto
{
    [ProtoMember(2)]
    public string Value { get; set; } = default!;

    public override FieldType FieldType => FieldType.Text;
}
