﻿using Persistify.Domain.Users;
using ProtoBuf;

namespace Persistify.Requests.Users;

[ProtoContract]
public class CreateUserRequest
{
    [ProtoMember(1)]
    public string Username { get; set; } = default!;
    [ProtoMember(2)]
    public string Password { get; set; } = default!;
    [ProtoMember(3)]
    public Role Role { get; set; }
}