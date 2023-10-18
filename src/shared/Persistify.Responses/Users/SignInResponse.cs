using Persistify.Dtos.Users;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class SignInResponse : IResponse
{
    [ProtoMember(1)]
    public UserDto User { get; set; } = default!;

    [ProtoMember(2)]
    public string AccessToken { get; set; } = default!;

    [ProtoMember(3)]
    public string RefreshToken { get; set; } = default!;
}
