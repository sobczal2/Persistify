using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class ExistsUserResponse
{
    [ProtoMember(1)]
    public bool Exists { get; set; }
}
