using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Number;

[ProtoContract]
public class RangeNumberSearchQuery : NumberSearchQuery
{
    [ProtoMember(11)]
    public double MinValue { get; set; }

    public double MaxValue { get; set; }
}
