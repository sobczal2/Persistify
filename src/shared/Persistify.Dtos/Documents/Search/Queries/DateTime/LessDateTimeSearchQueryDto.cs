using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.DateTime;

[ProtoContract]
public class LessDateTimeSearchQueryDto : DateTimeSearchQueryDto
{
    [ProtoMember(3)]
    public string FieldName { get; set; } = null!;

    [ProtoMember(2)]
    public System.DateTime Value { get; set; }

    public override string GetFieldName()
    {
        return FieldName;
    }

    public override void SetFieldName(
        string fieldName
    )
    {
        FieldName = fieldName;
    }
}
