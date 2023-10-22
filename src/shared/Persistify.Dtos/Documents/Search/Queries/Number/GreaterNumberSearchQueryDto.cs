﻿using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Number;

[ProtoContract]
public class GreaterNumberSearchQueryDto : NumberSearchQueryDto
{
    [ProtoMember(2)]
    public string FieldName { get; set; } = null!;

    [ProtoMember(3)]
    public double Value { get; set; }

    public override string GetFieldName()
    {
        return FieldName;
    }
}