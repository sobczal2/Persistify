using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Aggregates;

[ProtoContract]
public class AndSearchQueryDto : SearchQueryDto
{
    public AndSearchQueryDto()
    {
        SearchQueryDtos = new List<SearchQueryDto>();
    }

    [ProtoMember(2)]
    public List<SearchQueryDto> SearchQueryDtos { get; set; }
}
