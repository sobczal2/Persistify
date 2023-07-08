using System;
using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistify.Server.Configuration.Interceptors;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Server.Configuration.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddServicesConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpc(opt =>
        {
            var grpcSettings = configuration
                                   .GetRequiredSection(GrpcSettings.SectionName)
                                   .Get<GrpcSettings>() ??
                               throw new InvalidOperationException(
                                   $"Could not load {GrpcSettings.SectionName} from configuration");

            opt.ResponseCompressionLevel = Enum.Parse<CompressionLevel>(grpcSettings.ResponseCompressionLevel);
            opt.ResponseCompressionAlgorithm = grpcSettings.ResponseCompressionAlgorithm;
            opt.EnableDetailedErrors = grpcSettings.EnableDetailedErrors;
            opt.MaxReceiveMessageSize = grpcSettings.MaxReceiveMessageSize;
            opt.MaxSendMessageSize = grpcSettings.MaxSendMessageSize;
            opt.IgnoreUnknownServices = grpcSettings.IgnoreUnknownServices;
            
            opt.Interceptors.Add<CorrelationIdInterceptor>();
        });
        services.AddGrpcReflection();

        services.AddAuthorization();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var authSettings = configuration
                                       .GetRequiredSection(AuthSettings.SectionName)
                                       .Get<AuthSettings>() ??
                                   throw new InvalidOperationException(
                                       $"Could not load {AuthSettings.SectionName} from configuration");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = authSettings.ValidateIssuer,
                    ValidateAudience = authSettings.ValidateAudience,
                    ValidateLifetime = authSettings.ValidateLifetime,
                    ValidateIssuerSigningKey = authSettings.ValidateIssuerSigningKey,
                    ValidIssuer = authSettings.ValidIssuer,
                    ValidAudience = authSettings.ValidAudience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.IssuerSigningKey))
                };
            });

        services
            .AddDataProtection()
            .SetApplicationName("Persistify.Server")
            .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

        return services;
    }
}
