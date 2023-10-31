using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Bool;

[ProtoContract]
public class ExactBoolSearchQueryDto : BoolSearchQueryDto
{
    [ProtoMember(2)]
    public string FieldName { get; set; } = null!;

    [ProtoMember(3)]
    public bool Value { get; set; }

    public override string GetFieldName()
    {
        return FieldName;
    }

    public override void SetFieldName(string fieldName)
    {
        FieldName = fieldName;
    }
}
