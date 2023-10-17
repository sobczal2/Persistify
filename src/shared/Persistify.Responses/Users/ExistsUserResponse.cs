using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Users;

[ProtoContract]
public class ExistsUserResponse : IResponse
{
    [ProtoMember(1)]
    public bool Exists { get; set; }
}
