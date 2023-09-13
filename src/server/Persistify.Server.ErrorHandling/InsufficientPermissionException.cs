using System;
using Persistify.Domain.Users;

namespace Persistify.Server.ErrorHandling;

public class InsufficientPermissionException : Exception
{
    public InsufficientPermissionException() : base("Insufficient permission")
    {
    }

    public InsufficientPermissionException(Permission permission) : base(
        $"Insufficient permission. {(int)permission} required")
    {
    }
}
