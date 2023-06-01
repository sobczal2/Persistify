using System;
using System.IO.Compression;
using System.IO.Pipes;
using System.Reactive.Subjects;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistify.HostedServices;
using Persistify.Indexes.Boolean;
using Persistify.Indexes.Common;
using Persistify.Indexes.Number;
using Persistify.Indexes.Text;
using Persistify.Options;
using Persistify.Options.Storage;
using Persistify.Protos;
using Persistify.Server.Extensions.Documents;
using Persistify.Server.Extensions.Types;
using Persistify.Server.Interceptors;
using Persistify.Storage;
using Persistify.Stores.Documents;
using Persistify.Stores.Types;
using Persistify.Stores.User;
using Persistify.Tokens;
using Persistify.Validators.Core;
using Serilog;
using AuthService = Persistify.Server.Services.AuthService;
using DeflateCompressionProvider = Grpc.Net.Compression.DeflateCompressionProvider;
using DocumentService = Persistify.Server.Services.DocumentService;
using MonitorService = Persistify.Server.Services.MonitorService;
using TypeService = Persistify.Server.Services.TypeService;

namespace Persistify.Server;

public static class PersistifyExtensions
{
    public static IServiceCollection AddPersistify(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAndValidateOptions(configuration);
        services.AddGrpc(opt =>
        {
            opt.Interceptors.Add<ValidationInterceptor>();
            // TODO: add grpc options
        });
        services.AddGrpcReflection();
        services.AddAuthorization();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                var securityOptions = services
                    .BuildServiceProvider()
                    .GetRequiredService<IOptions<SecurityOptions>>()
                    .Value;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = securityOptions.Token.Issuer,
                    ValidAudience = securityOptions.Token.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(securityOptions.Token.Key)
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });
        services.AddValidators();

        services.AddPersistifyPipeline();
        services.AddPersistifyStores();
        services.AddPersistifyIndexers();
        services.AddPersistifySubjects();
        services.AddOtherPersistifyServices();

        return services;
    }

    public static WebApplication UsePersistify(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.MapGrpcReflectionService();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGrpcService<TypeService>();
        app.MapGrpcService<DocumentService>();
        app.MapGrpcService<AuthService>();
        app.MapGrpcService<MonitorService>();

        return app;
    }

    private static IServiceCollection AddPersistifyPipeline(this IServiceCollection services)
    {
        services.AddIndexDocumentEndpoint();
        services.AddRemoveDocumentEndpoint();
        services.AddComplexSearchDocumentsEndpoint();

        services.AddCreateTypeEndpoint();
        services.AddListTypesEndpoint();

        return services;
    }

    private static IServiceCollection AddPersistifyIndexers(this IServiceCollection services)
    {
        var textIndexer = new TextIndexer();
        services.AddSingleton<IIndexer<string>>(textIndexer);
        services.AddSingleton<IPersisted>(textIndexer);

        var numberIndexer = new NumberIndexer();
        services.AddSingleton<IIndexer<double>>(numberIndexer);
        services.AddSingleton<IPersisted>(numberIndexer);

        var booleanIndexer = new BooleanIndexer();
        services.AddSingleton<IIndexer<bool>>(booleanIndexer);
        services.AddSingleton<IPersisted>(booleanIndexer);

        return services;
    }

    private static IServiceCollection AddPersistifyStores(this IServiceCollection services)
    {
        services.AddSingleton<IStorage>(sp =>
        {
            var storageOptions = sp
                .GetRequiredService<IOptions<StorageOptions>>()
                .Value;
            
            return storageOptions.Type switch
            {
                "InMemory" => new InMemoryStorage(),
                "Files" => new FilesStorage(storageOptions.Path!),
                "GzipFiles" => new GzipFilesStorage(storageOptions.Path!),
                _ => throw new InvalidOperationException()
            };
        });

        services.AddSingleton<ITypeStore>(_ => new HashSetTypeStore());
        services.AddSingleton<IPersisted>(
            sp =>
                sp.GetRequiredService<ITypeStore>() as IPersisted
                ?? throw new InvalidOperationException()
        );

        services.AddSingleton<IDocumentStore>(sp =>
        {
            var storage = sp.GetRequiredService<IStorage>();
            return new StorageDocumentStore(storage);
        });
        services.AddSingleton<IPersisted>(
            sp =>
                sp.GetRequiredService<IDocumentStore>() as IPersisted
                ?? throw new InvalidOperationException()
        );

        services.AddSingleton<IUserStore>(sp => new UserStore(sp.GetRequiredService<IOptions<SecurityOptions>>()));
        services.AddSingleton<IPersisted>(
            sp =>
                sp.GetRequiredService<IUserStore>() as IPersisted
                ?? throw new InvalidOperationException()
        );

        services.AddHostedService<PersistedHostedService>();

        return services;
    }

    public static IServiceCollection AddPersistifySubjects(this IServiceCollection services)
    {
        services.AddSingleton<ISubject<PipelineEventProto>, Subject<PipelineEventProto>>();

        return services;
    }

    private static IServiceCollection AddOtherPersistifyServices(this IServiceCollection services)
    {
        services.AddSingleton<ITokenizer, Tokenizer>();

        return services;
    }
}