using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Number;

[ProtoContract]
public class RangeNumberSearchQuery : NumberSearchQuery
{
    [ProtoMember(2)]
    public string FieldName { get; set; } = null!;

    [ProtoMember(3)]
    public double MinValue { get; set; }

    [ProtoMember(4)]
    public double MaxValue { get; set; }

    public override string GetFieldName()
    {
        return FieldName;
    }
}
