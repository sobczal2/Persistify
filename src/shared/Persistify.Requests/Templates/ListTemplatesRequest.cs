using Persistify.Requests.Common;
using Persistify.Responses.Templates;
using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class ListTemplatesRequest : IRequest<ListTemplatesResponse>
{
    [ProtoMember(1)]
    public Pagination Pagination { get; set; } = default!;
}
