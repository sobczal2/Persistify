using Persistify.Requests.Common;
using Persistify.Responses.Users;
using ProtoBuf;

namespace Persistify.Requests.Users;

[ProtoContract]
public class ExistsUserRequest : IRequest<ExistsUserResponse>
{
    [ProtoMember(1)]
    public string Username { get; set; } = default!;
}
