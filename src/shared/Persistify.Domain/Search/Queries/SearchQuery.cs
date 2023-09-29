using Persistify.Domain.Search.Queries.Aggregates;
using Persistify.Domain.Search.Queries.Bool;
using Persistify.Domain.Search.Queries.Number;
using Persistify.Domain.Search.Queries.Text;
using ProtoBuf;

namespace Persistify.Domain.Search.Queries;

[ProtoContract]
[ProtoInclude(31, typeof(AndSearchQuery))]
[ProtoInclude(32, typeof(NotSearchQuery))]
[ProtoInclude(33, typeof(OrSearchQuery))]
[ProtoInclude(41, typeof(BoolSearchQuery))]
[ProtoInclude(51, typeof(EqualNumberSearchQuery))]
[ProtoInclude(52, typeof(GreaterNumberSearchQuery))]
[ProtoInclude(53, typeof(LessNumberSearchQuery))]
[ProtoInclude(54, typeof(RangeNumberSearchQuery))]
[ProtoInclude(61, typeof(ExactTextSearchQuery))]
[ProtoInclude(62, typeof(FullTextSearchQuery))]
[ProtoInclude(63, typeof(PrefixTextSearchQuery))]
public abstract class SearchQuery
{
    public SearchQuery()
    {
        Boost = 1f;
    }

    [ProtoMember(1)]
    public float Boost { get; set; }
}
