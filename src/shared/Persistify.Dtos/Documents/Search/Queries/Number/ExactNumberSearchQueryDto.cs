using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Number;

[ProtoContract]
public class ExactNumberSearchQueryDto : NumberSearchQueryDto
{
    [ProtoMember(2)]
    public double Value { get; set; }

    [ProtoMember(3)]
    public string FieldName { get; set; } = null!;

    public override string GetFieldName()
    {
        return FieldName;
    }
}
