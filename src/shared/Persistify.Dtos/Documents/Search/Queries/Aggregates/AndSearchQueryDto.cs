using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Dtos.Documents.Search.Queries.Aggregates;

[ProtoContract]
public class AndSearchQueryDto : SearchQueryDto
{
    public AndSearchQueryDto()
    {
        Queries = new List<SearchQueryDto>();
    }

    [ProtoMember(2)]
    public List<SearchQueryDto> Queries { get; set; }
}
