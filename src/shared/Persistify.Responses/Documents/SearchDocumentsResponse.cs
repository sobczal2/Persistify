using System.Collections.Generic;
using Persistify.Domain.Search;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class SearchDocumentsResponse
{
    [ProtoMember(1)]
    public IEnumerable<SearchRecord> SearchRecords { get; set; } = default!;

    [ProtoMember(2)]
    public int TotalCount { get; set; }
}
