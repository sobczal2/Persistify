using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Codes;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class InsufficientPermissionPersistifyException : PersistifyException
{
    public InsufficientPermissionPersistifyException(
        string? requestName,
        Permission requiredPermission
    )
        : base(
            requestName ?? "Unknown",
            $"Insufficient permission. Required permission: {requiredPermission}",
            PersistifyErrorCode.InsufficientPermission
        )
    {
    }
}
