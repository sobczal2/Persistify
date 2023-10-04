using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Aggregates;

[ProtoContract]
public class AndSearchQuery : SearchQuery
{
    public AndSearchQuery()
    {
        Queries = new List<SearchQuery>();
    }

    [ProtoMember(2)]
    public List<SearchQuery> Queries { get; set; }
}
