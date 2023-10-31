using System.Collections.Generic;
using System.Security.Claims;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Exceptions;

namespace Persistify.Server.Security;

public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal InternalClaimsPrincipal =>
        GetClaimsPrincipal(0, "Internal", Permission.All);

    public static ClaimsPrincipal UnknownClaimsPrincipal =>
        GetClaimsPrincipal(-1, "Unknown", Permission.None);

    public static Permission GetPermission(this ClaimsPrincipal claimsPrincipal)
    {
        var permissionClaim = claimsPrincipal.FindFirst(ClaimTypes.Permission);

        if (permissionClaim is null)
        {
            return Permission.None;
        }

        if (!int.TryParse(permissionClaim.Value, out var permissionInt))
        {
            throw new InternalPersistifyException();
        }

        return (Permission)permissionInt;
    }

    public static ClaimsPrincipal GetClaimsPrincipal(int id, string username, Permission permission)
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.Id, id.ToString()),
                    new(ClaimTypes.Username, username),
                    new(ClaimTypes.Permission, ((int)permission).ToString())
                }
            )
        );
    }
}
