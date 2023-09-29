using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Number;

[ProtoContract]
public class EqualNumberSearchQuery : NumberSearchQuery
{
    [ProtoMember(11)]
    public double Value { get; set; }
}
