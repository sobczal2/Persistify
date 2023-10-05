using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Text;

[ProtoContract]
public class FullTextSearchQuery : TextSearchQuery
{
    [ProtoMember(2)]
    public string Value { get; set; } = null!;

    [ProtoMember(3)]
    public string FieldName { get; set; } = null!;

    public override string GetFieldName()
    {
        return FieldName;
    }
}
