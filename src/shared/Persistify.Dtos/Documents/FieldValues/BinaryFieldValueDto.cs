using Persistify.Dtos.Common;
using ProtoBuf;

namespace Persistify.Dtos.Documents.FieldValues;

[ProtoContract]
public class BinaryFieldValueDto : FieldValueDto
{
    [ProtoMember(2)]
    public byte[] Value { get; set; } = default!;

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.Binary;
}
