using Persistify.Domain.Search.Queries;
using Persistify.Requests.Common;
using Persistify.Responses.Documents;
using ProtoBuf;

namespace Persistify.Requests.Documents;

[ProtoContract]
public class SearchDocumentsRequest : IRequest<SearchDocumentsResponse>
{
    [ProtoMember(1)]
    public string TemplateName { get; set; } = default!;

    [ProtoMember(2)]
    public Pagination Pagination { get; set; } = default!;

    [ProtoMember(3)]
    public SearchQuery SearchQuery { get; set; } = default!;
}
