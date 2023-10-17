using System.Collections.Generic;
using Persistify.Dtos.Users;
using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class ListUsersResponse
{
    [ProtoMember(1)]
    public List<UserDto> Users { get; set; } = default!;

    [ProtoMember(2)]
    public int TotalCount { get; set; }
}
