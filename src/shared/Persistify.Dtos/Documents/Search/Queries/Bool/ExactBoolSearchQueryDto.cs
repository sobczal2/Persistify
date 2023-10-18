using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Bool;

[ProtoContract]
public class ExactBoolSearchQueryDto : BoolSearchQueryDto
{
    [ProtoMember(2)]
    public bool Value { get; set; }

    [ProtoMember(3)]
    public string FieldName { get; set; } = null!;

    public override string GetFieldName()
    {
        return FieldName;
    }
}
