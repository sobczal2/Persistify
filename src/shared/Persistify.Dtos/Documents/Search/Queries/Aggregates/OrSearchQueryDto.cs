using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Aggregates;

[ProtoContract]
public class OrSearchQueryDto : SearchQueryDto
{
    public OrSearchQueryDto()
    {
        SearchQueryDtos = new List<SearchQueryDto>();
    }

    [ProtoMember(2)]
    public List<SearchQueryDto> SearchQueryDtos { get; set; }
}
