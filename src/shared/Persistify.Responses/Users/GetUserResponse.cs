using Persistify.Dtos.Users;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class GetUserResponse : IResponse
{
    [ProtoMember(1)]
    public UserDto User { get; set; } = default!;
}
