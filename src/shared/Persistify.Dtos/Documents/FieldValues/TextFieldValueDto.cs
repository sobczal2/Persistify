using Persistify.Server.Domain.Templates;
using ProtoBuf;

namespace Persistify.Dtos.Documents.FieldValues;

[ProtoContract]
public class TextFieldValueDto : FieldValueDto
{
    [ProtoMember(2)]
    public string Value { get; set; } = default!;

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.Text;
}
