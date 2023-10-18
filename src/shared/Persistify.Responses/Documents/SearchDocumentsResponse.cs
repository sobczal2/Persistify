using System.Collections.Generic;
using Persistify.Dtos.Documents.Search;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class SearchDocumentsResponse : IResponse
{
    [ProtoMember(1)]
    public List<SearchRecordDto> SearchRecords { get; set; } = default!;

    [ProtoMember(2)]
    public int TotalCount { get; set; }
}
