using ProtoBuf;

namespace Persistify.Requests.Users;

[ProtoContract]
public class SetPermissionRequest
{
    [ProtoMember(1)] public string Username { get; set; } = default!;

    [ProtoMember(2)] public int Permission { get; set; }
}
