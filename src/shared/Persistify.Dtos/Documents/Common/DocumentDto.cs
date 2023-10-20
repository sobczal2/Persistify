using System.Collections.Generic;
using Persistify.Dtos.Documents.FieldValues;
using ProtoBuf;

namespace Persistify.Dtos.Documents.Common;

[ProtoContract]
public class DocumentDto
{
    public DocumentDto()
    {
        FieldValues = new List<FieldValueDto>();
    }

    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public List<FieldValueDto> FieldValues { get; set; }
}
