using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.DateTime;

[ProtoContract]
public class RangeDateTimeSearchQueryDto : DateTimeSearchQueryDto
{
    [ProtoMember(2)]
    public string FieldName { get; set; } = null!;

    [ProtoMember(3)]
    public System.DateTime MinValue { get; set; }

    [ProtoMember(4)]
    public System.DateTime MaxValue { get; set; }

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
