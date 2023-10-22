using Persistify.Dtos.Common;
using ProtoBuf;

namespace Persistify.Dtos.Documents.FieldValues;

[ProtoContract]
public class NumberFieldValueDto : FieldValueDto
{
    [ProtoMember(2)]
    public double Value { get; set; }

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;
}
