using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class GetUserResponse : IResponse
{
    [ProtoMember(1)]
    public string Username { get; set; } = default!;

    [ProtoMember(2)]
    public int Permission { get; set; }
}
