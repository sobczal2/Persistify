using Persistify.Dtos.Common;
using Persistify.Requests.Common;
using Persistify.Responses.Templates;
using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class ListTemplatesRequest : IRequest<ListTemplatesResponse>
{
    [ProtoMember(1)]
    public PaginationDto PaginationDto { get; set; } = default!;
}
