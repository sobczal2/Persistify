using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Text;

[ProtoContract]
public class FullTextSearchQueryDto : TextSearchQueryDto
{
    [ProtoMember(2)]
    public string Value { get; set; } = null!;

    [ProtoMember(3)]
    public string FieldName { get; set; } = null!;

    public override string GetFieldName()
    {
        return FieldName;
    }

    public override void SetFieldName(string fieldName)
    {
        FieldName = fieldName;
    }
}
