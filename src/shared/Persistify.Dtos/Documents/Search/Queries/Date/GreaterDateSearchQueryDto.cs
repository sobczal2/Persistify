using System;
using Persistify.Dtos.Documents.Search.Queries.Date;
using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Number;

[ProtoContract]
public class GreaterDateSearchQueryDto : DateSearchQueryDto
{
    [ProtoMember(2)]
    public string FieldName { get; set; } = null!;

    [ProtoMember(3)]
    public DateTime Value { get; set; }

    public override string GetFieldName()
    {
        return FieldName;
    }

    public override void SetFieldName(string fieldName)
    {
        FieldName = fieldName;
    }
}
