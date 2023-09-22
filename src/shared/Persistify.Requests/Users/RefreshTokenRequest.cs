using ProtoBuf;

namespace Persistify.Requests.Users;

[ProtoContract]
public class RefreshTokenRequest
{
    [ProtoMember(1)] public string Username { get; set; } = default!;

    [ProtoMember(2)] public string RefreshToken { get; set; } = default!;
}
