using Persistify.Dtos.Documents.Search.Queries.Aggregates;
using Persistify.Dtos.Documents.Search.Queries.Bool;
using Persistify.Dtos.Documents.Search.Queries.Number;
using Persistify.Dtos.Documents.Search.Queries.Text;
using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries;

[ProtoContract]
[ProtoInclude(100, typeof(AndSearchQueryDto))]
[ProtoInclude(101, typeof(NotSearchQueryDto))]
[ProtoInclude(102, typeof(OrSearchQueryDto))]
[ProtoInclude(200, typeof(ExactBoolSearchQueryDto))]
[ProtoInclude(300, typeof(ExactNumberSearchQueryDto))]
[ProtoInclude(301, typeof(GreaterNumberSearchQueryDto))]
[ProtoInclude(302, typeof(LessNumberSearchQueryDto))]
[ProtoInclude(303, typeof(RangeNumberSearchQueryDto))]
[ProtoInclude(400, typeof(ExactTextSearchQueryDto))]
[ProtoInclude(401, typeof(FullTextSearchQueryDto))]
public class SearchQueryDto
{
    [ProtoMember(1)]
    public float Boost { get; set; }
}
