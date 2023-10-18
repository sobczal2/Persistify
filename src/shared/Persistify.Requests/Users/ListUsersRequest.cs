using Persistify.Dtos.Common;
using Persistify.Requests.Common;
using Persistify.Responses.Users;
using ProtoBuf;

namespace Persistify.Requests.Users;

[ProtoContract]
public class ListUsersRequest : IRequest<ListUsersResponse>
{
    [ProtoMember(1)]
    public PaginationDto Pagination { get; set; } = default!;
}
