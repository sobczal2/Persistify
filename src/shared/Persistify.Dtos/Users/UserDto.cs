using ProtoBuf;

namespace Persistify.Dtos.Users;

[ProtoContract]
public class UserDto
{
    [ProtoMember(1)]
    public string Username { get; set; } = default!;

    [ProtoMember(2)]
    public int Permission { get; set; }
}
