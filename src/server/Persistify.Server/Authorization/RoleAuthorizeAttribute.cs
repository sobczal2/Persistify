using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Persistify.Domain.Users;

namespace Persistify.Server.Authorization;

public class RoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly Role _role;

    public RoleAuthorizeAttribute(Role role)
    {
        _role = role;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userRole = Enum.Parse<Role>(user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty);

        if (userRole.HasFlag(_role))
        {
            return;
        }

        context.Result = new ForbidResult();
    }
}
