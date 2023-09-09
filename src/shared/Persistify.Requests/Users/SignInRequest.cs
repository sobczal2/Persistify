using ProtoBuf;

namespace Persistify.Requests.Users;

[ProtoContract]
public class SignInRequest
{
    [ProtoMember(1)]
    public string Username { get; set; } = default!;
    [ProtoMember(2)]
    public string Password { get; set; } = default!;
}
