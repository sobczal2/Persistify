using System;
using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Persistify.Helpers;
using Persistify.Helpers.Time;
using Persistify.Server.CommandHandlers;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Fts;
using Persistify.Server.HostedServices;
using Persistify.Server.Management;
using Persistify.Server.Security;
using Persistify.Server.Serialization;
using Persistify.Server.Validation;
using ProtoBuf.Grpc.Server;

namespace Persistify.Server.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddServicesConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<HostOptions>(opt => opt.ShutdownTimeout = TimeSpan.FromMinutes(1));
        services.AddCodeFirstGrpc();
        services.AddCodeFirstGrpcReflection();
        services.AddGrpc(opt =>
        {
            var grpcSettings =
                configuration.GetRequiredSection(GrpcSettings.SectionName).Get<GrpcSettings>()
                ?? throw new InvalidOperationException(
                    $"Could not load {GrpcSettings.SectionName} from configuration"
                );

            opt.ResponseCompressionLevel = Enum.Parse<CompressionLevel>(
                grpcSettings.ResponseCompressionLevel
            );
            opt.ResponseCompressionAlgorithm = grpcSettings.ResponseCompressionAlgorithm;
            opt.EnableDetailedErrors = grpcSettings.EnableDetailedErrors;
            opt.MaxReceiveMessageSize = grpcSettings.MaxReceiveMessageSize;
            opt.MaxSendMessageSize = grpcSettings.MaxSendMessageSize;
            opt.IgnoreUnknownServices = grpcSettings.IgnoreUnknownServices;
        });

        services.AddAuthorization();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    var tokenSettings =
                        configuration
                            .GetRequiredSection(TokenSettings.SectionName)
                            .Get<TokenSettings>()
                        ?? throw new InvalidOperationException(
                            $"Could not load {TokenSettings.SectionName} from configuration"
                        );

                    var clock = services.BuildServiceProvider().GetRequiredService<IClock>();

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        LifetimeValidator = (
                            notBefore,
                            expires,
                            securityToken,
                            validationParameters
                        ) =>
                        {
                            var now = clock.UtcNow;
                            return notBefore <= now && expires >= now;
                        },
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(tokenSettings.Secret)
                        ),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,
                        ClockSkew = TimeSpan.Zero
                    };
                }
            );

        services.AddSerialization(configuration);
        services.AddValidation();
        services.AddCommands();
        services.AddManagement();
        services.AddHostedServices();
        services.AddErrorHandling();
        services.AddSecurity();
        services.AddHelpers();
        services.AddFtsAnalysis();

        return services;
    }
}
