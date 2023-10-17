using System.Collections.Generic;
using Persistify.Domain.Search;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class SearchDocumentsResponse : IResponse
{
    [ProtoMember(1)]
    public List<SearchRecord> SearchRecords { get; set; } = default!;

    [ProtoMember(2)]
    public int TotalCount { get; set; }
}
