using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Aggregates;

[ProtoContract]
public class NotSearchQuery : SearchQuery
{
    public NotSearchQuery()
    {
        Queries = Array.Empty<SearchQuery>();
    }

    [ProtoMember(11)]
    public IEnumerable<SearchQuery> Queries { get; set; }
}
