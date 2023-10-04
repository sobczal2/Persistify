using Persistify.Domain.Search.Queries.Aggregates;
using Persistify.Domain.Search.Queries.Bool;
using Persistify.Domain.Search.Queries.Number;
using Persistify.Domain.Search.Queries.Text;
using ProtoBuf;

namespace Persistify.Domain.Search.Queries;

[ProtoContract]
[ProtoInclude(100, typeof(AndSearchQuery))]
[ProtoInclude(101, typeof(NotSearchQuery))]
[ProtoInclude(102, typeof(OrSearchQuery))]
[ProtoInclude(200, typeof(ExactBoolSearchQuery))]
[ProtoInclude(300, typeof(ExactNumberSearchQuery))]
[ProtoInclude(301, typeof(GreaterNumberSearchQuery))]
[ProtoInclude(302, typeof(LessNumberSearchQuery))]
[ProtoInclude(303, typeof(RangeNumberSearchQuery))]
[ProtoInclude(400, typeof(ExactTextSearchQuery))]
[ProtoInclude(401, typeof(FullTextSearchQuery))]
[ProtoInclude(402, typeof(PrefixTextSearchQuery))]
public class SearchQuery
{
    [ProtoMember(1)]
    public float Boost { get; set; }
}
