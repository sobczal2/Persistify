using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Aggregates;

[ProtoContract]
public class OrSearchQuery : SearchQuery
{
    public OrSearchQuery()
    {
        Queries = new List<SearchQuery>();
    }

    [ProtoMember(2)]
    public List<SearchQuery> Queries { get; set; }
}
