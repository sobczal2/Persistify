using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Aggregates;

[ProtoContract]
public class NotSearchQuery : SearchQuery
{
    [ProtoMember(2)]
    public SearchQuery Query { get; set; } = default!;
}
