using System;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Fts.Analysis;
using Persistify.Server.HostedServices;
using Persistify.Server.Management.Domain;
using Persistify.Server.Persistence.Core;
using Persistify.Server.Persistence.DataStructures;
using Persistify.Server.Pipelines;
using Persistify.Server.Pipelines.Documents;
using Persistify.Server.Pipelines.Templates;
using Persistify.Server.Serialization;
using Persistify.Server.Validation;
using ProtoBuf.Grpc.Server;

namespace Persistify.Server.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddServicesConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<HostOptions>(opt => opt.ShutdownTimeout = TimeSpan.FromMinutes(1));
        services.AddCodeFirstGrpc();
        services.AddCodeFirstGrpcReflection();
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
        });

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

        services.AddSerialization(configuration);
        services.AddValidation();
        services.AddPipelines();
        services.AddPipelinesTemplates();
        services.AddPipelinesDocuments();
        services.AddFtsAnalysis();
        services.AddManagement();
        services.AddPersistenceCore(configuration);
        services.AddHostedServices(AppDomain.CurrentDomain.GetAssemblies().Where(assembly =>
            assembly.FullName?.StartsWith("Persistify") ?? false).ToArray());

        return services;
    }
}
