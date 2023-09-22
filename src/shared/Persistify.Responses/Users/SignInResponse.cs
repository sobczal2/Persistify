using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class SignInResponse
{
    [ProtoMember(1)] public string Username { get; set; } = default!;

    [ProtoMember(2)] public int Permission { get; set; }

    [ProtoMember(3)] public string AccessToken { get; set; } = default!;

    [ProtoMember(4)] public string RefreshToken { get; set; } = default!;
}
