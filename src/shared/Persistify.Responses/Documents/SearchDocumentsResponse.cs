using System.Collections.Generic;
using Persistify.Dtos.Documents.Search;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class SearchDocumentsResponse : IResponse
{
    public SearchDocumentsResponse()
    {
        SearchRecordDtos = new List<SearchRecordDto>();
    }

    [ProtoMember(1)]
    public List<SearchRecordDto> SearchRecordDtos { get; set; } = default!;

    [ProtoMember(2)]
    public int TotalCount { get; set; }
}
