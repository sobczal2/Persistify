using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries;
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
    public PaginationDto Pagination { get; set; } = default!;

    [ProtoMember(3)]
    public SearchQueryDto SearchQuery { get; set; } = default!;
}
