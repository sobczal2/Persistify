using ProtoBuf;

namespace Persistify.Requests.Users;

[ProtoContract]
public class GetUserRequest
{
    [ProtoMember(1)]
    public string Username { get; set; } = default!;
}
