using System.Collections.Generic;
using Persistify.Dtos.Users;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class ListUsersResponse : IResponse
{
    [ProtoMember(1)]
    public List<UserDto> Users { get; set; }

    [ProtoMember(2)]
    public int TotalCount { get; set; }

    public ListUsersResponse()
    {
        Users = new List<UserDto>();
    }
}
