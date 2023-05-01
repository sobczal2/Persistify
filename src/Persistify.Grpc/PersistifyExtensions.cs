using System;
using System.IO.Compression;
using System.IO.Pipes;
using System.Reactive.Subjects;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistify.Grpc.Extensions.Documents;
using Persistify.Grpc.Extensions.Types;
using Persistify.Grpc.Interceptors;
using Persistify.HostedServices;
using Persistify.Indexes.Boolean;
using Persistify.Indexes.Common;
using Persistify.Indexes.Number;
using Persistify.Indexes.Text;
using Persistify.Options;
using Persistify.Protos;
using Persistify.Storage;
using Persistify.Stores.Documents;
using Persistify.Stores.Types;
using Persistify.Stores.User;
using Persistify.Tokens;
using Persistify.Validators.Types;
using Serilog;
using AuthService = Persistify.Grpc.Services.AuthService;
using DeflateCompressionProvider = Grpc.Net.Compression.DeflateCompressionProvider;
using DocumentService = Persistify.Grpc.Services.DocumentService;
using TypeService = Persistify.Grpc.Services.TypeService;
using MonitorService = Persistify.Grpc.Services.MonitorService;

namespace Persistify.Grpc;

public static class PersistifyExtensions
{
    public static IServiceCollection AddPersistify(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(opt =>
        {
            opt.Interceptors.Add<ValidationInterceptor>();
            opt.CompressionProviders.Add(new DeflateCompressionProvider(CompressionLevel.SmallestSize));
        });
        services.AddGrpcReflection();
        services.AddPersistifyOptions(configuration);
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection(AuthOptions.SectionName).GetValue<string>("Issuer"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration
                            .GetSection(AuthOptions.SectionName).GetValue<string>("JwtSecret") ??
                        throw new InvalidOperationException())),
                    ClockSkew = TimeSpan.Zero
                };
            });
        services.AddValidatorsFromAssemblyContaining<CreateTypeRequestProtoValidator>(ServiceLifetime.Singleton);

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
        services.AddSearchDocumentsEndpoint();
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
        services.AddSingleton<IStorage>(_ => new GzipFileSystemStorage("/home/sobczal/temp"));

        services.AddSingleton<ITypeStore>(_ => new HashSetTypeStore());
        services.AddSingleton<IPersisted>(sp =>
            sp.GetRequiredService<ITypeStore>() as IPersisted ?? throw new InvalidOperationException());

        services.AddSingleton<IDocumentStore>(sp =>
        {
            var storage = sp.GetRequiredService<IStorage>();
            return new StorageDocumentStore(storage);
        });
        services.AddSingleton<IPersisted>(sp =>
            sp.GetRequiredService<IDocumentStore>() as IPersisted ?? throw new InvalidOperationException());

        services.AddSingleton<IUserStore>(sp =>
        {
            var authOptionsMonitor = sp.GetRequiredService<IOptionsMonitor<AuthOptions>>();
            return new UserStore(authOptionsMonitor);
        });
        services.AddSingleton<IPersisted>(sp =>
            sp.GetRequiredService<IUserStore>() as IPersisted ?? throw new InvalidOperationException());


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

        services.AddSingleton(_ => new NamedPipeServerStream("monitoring-pipe", PipeDirection.Out, 1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous));

        return services;
    }
}