using Persistify.Requests.Shared;
using ProtoBuf;

namespace Persistify.Requests.Users;

[ProtoContract]
public class ListUsersRequest
{
    [ProtoMember(1)]
    public Pagination Pagination { get; set; } = default!;
}
