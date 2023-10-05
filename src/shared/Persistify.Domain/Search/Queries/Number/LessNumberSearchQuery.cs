using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Number;

[ProtoContract]
public class LessNumberSearchQuery : NumberSearchQuery
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
