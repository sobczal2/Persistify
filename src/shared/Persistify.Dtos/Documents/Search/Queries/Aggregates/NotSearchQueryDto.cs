using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Aggregates;

[ProtoContract]
public class NotSearchQueryDto : SearchQueryDto
{
    [ProtoMember(2)]
    public SearchQueryDto QueryDto { get; set; } = default!;
}
