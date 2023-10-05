using Persistify.Domain.Search.Queries;
using Persistify.Requests.Shared;
using ProtoBuf;

namespace Persistify.Requests.Documents;

[ProtoContract]
public class SearchDocumentsRequest
{
    [ProtoMember(1)]
    public string TemplateName { get; set; } = default!;

    [ProtoMember(2)]
    public Pagination Pagination { get; set; } = default!;

    [ProtoMember(3)]
    public SearchQuery SearchQuery { get; set; } = default!;
}
