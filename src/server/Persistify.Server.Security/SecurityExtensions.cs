﻿using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Server.Security;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services
    )
    {
        services.AddSingleton<IPasswordService, Argon2PasswordService>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
