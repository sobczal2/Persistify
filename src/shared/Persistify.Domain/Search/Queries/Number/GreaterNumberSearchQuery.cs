using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Number;

[ProtoContract]
public class GreaterNumberSearchQuery : NumberSearchQuery
{
    [ProtoMember(11)]
    public double Value { get; set; }
}
