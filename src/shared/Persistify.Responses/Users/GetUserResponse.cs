using Persistify.Domain.Users;
using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class GetUserResponse
{
    [ProtoMember(1)]
    public string Username { get; set; } = default!;
    [ProtoMember(2)]
    public Role Role { get; set; }
}
