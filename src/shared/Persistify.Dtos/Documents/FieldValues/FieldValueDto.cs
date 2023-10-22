using System;
using Persistify.Dtos.Common;
using ProtoBuf;

namespace Persistify.Dtos.Documents.FieldValues;

[ProtoContract]
[ProtoInclude(100, typeof(BoolFieldValueDto))]
[ProtoInclude(101, typeof(NumberFieldValueDto))]
[ProtoInclude(102, typeof(TextFieldValueDto))]
public class FieldValueDto
{
    [ProtoMember(1)]
    public string FieldName { get; set; } = default!;

    public virtual FieldTypeDto FieldTypeDto => throw new NotImplementedException();
}
