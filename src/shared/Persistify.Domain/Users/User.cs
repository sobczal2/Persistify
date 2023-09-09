using ProtoBuf;

namespace Persistify.Domain.Users;

[ProtoContract]
public class User
{
    [ProtoMember(1)]
    public int Id { get; set; }
    [ProtoMember(2)]
    public string Username { get; set; } = default!;
    [ProtoMember(3)]
    public byte[] PasswordHash { get; set; } = default!;
    [ProtoMember(4)]
    public Role Role { get; set; }
}
