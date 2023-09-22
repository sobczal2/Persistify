using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class RefreshTokenResponse
{
    [ProtoMember(1)] public string AccessToken { get; set; } = default!;

    [ProtoMember(2)] public string RefreshToken { get; set; } = default!;
}
