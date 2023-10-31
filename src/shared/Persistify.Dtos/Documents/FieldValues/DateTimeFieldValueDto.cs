using System;
using Persistify.Dtos.Common;
using ProtoBuf;

namespace Persistify.Dtos.Documents.FieldValues;

[ProtoContract]
public class DateTimeFieldValueDto : FieldValueDto
{
    [ProtoMember(2)]
    public DateTime Value { get; set; }

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.DateTime;
}
