using System;
using ProtoBuf;

namespace Persistify.Server.Domain.Users;

[ProtoContract]
public class RefreshToken
{
    [ProtoMember(1)]
    public string Value { get; set; } = default!;

    [ProtoMember(2)]
    public DateTime Created { get; set; }

    [ProtoMember(3)]
    public DateTime Expires { get; set; }
}
