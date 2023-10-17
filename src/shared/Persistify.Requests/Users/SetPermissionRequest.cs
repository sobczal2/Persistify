using Persistify.Requests.Common;
using Persistify.Responses.Users;
using ProtoBuf;

namespace Persistify.Requests.Users;

[ProtoContract]
public class SetPermissionRequest : IRequest<SetPermissionResponse>
{
    [ProtoMember(1)]
    public string Username { get; set; } = default!;

    [ProtoMember(2)]
    public int Permission { get; set; }
}
