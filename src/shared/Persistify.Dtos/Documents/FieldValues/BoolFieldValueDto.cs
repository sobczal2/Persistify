using Persistify.Server.Domain.Templates;
using ProtoBuf;

namespace Persistify.Dtos.Documents.FieldValues;

[ProtoContract]
public class BoolFieldValueDto : FieldValueDto
{
    [ProtoMember(2)]
    public bool Value { get; set; }

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.Bool;
}
