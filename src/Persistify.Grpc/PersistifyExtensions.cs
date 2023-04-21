using System.IO.Compression;
using FluentValidation;
using Grpc.Net.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Grpc.Extensions.Documents;
using Persistify.Grpc.Extensions.Types;
using Persistify.Grpc.Interceptors;
using Persistify.Grpc.Services;
using Persistify.HostedServices;
using Persistify.Indexes.Boolean;
using Persistify.Indexes.Common;
using Persistify.Indexes.Number;
using Persistify.Indexes.Text;
using Persistify.Storage;
using Persistify.Stores.Documents;
using Persistify.Stores.Types;
using Persistify.Tokens;
using Persistify.Validators.Types;
using Serilog;

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

        services.AddPipeline();
        services.AddValidatorsFromAssemblyContaining<CreateTypeRequestProtoValidator>(ServiceLifetime.Singleton);
        services.AddStores();
        services.AddIndexers();
        services.AddOtherServices();

        return services;
    }

    public static WebApplication UsePersistify(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.MapGrpcReflectionService();

        app.MapGrpcService<TypeService>();
        app.MapGrpcService<DocumentService>();

        app.MapGet("/", () => "Use gRPC client to call the service");

        return app;
    }

    private static IServiceCollection AddPipeline(this IServiceCollection services)
    {
        services.AddIndexDocumentEndpoint();
        services.AddRemoveDocumentEndpoint();
        services.AddSearchDocumentsEndpoint();
        
        services.AddCreateTypeEndpoint();
        services.AddListTypesEndpoint();

        return services;
    }
    
    private static IServiceCollection AddIndexers(this IServiceCollection services)
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

    private static IServiceCollection AddStores(this IServiceCollection services)
    {
        var fileStorage = new GzipFileSystemStorage("/home/sobczal/temp");
        services.AddSingleton<IStorage>(fileStorage);
        
        var typeStore = new HashSetTypeStore();
        services.AddSingleton<ITypeStore>(typeStore);
        services.AddSingleton<IPersisted>(typeStore);

        var documentStore = new StorageDocumentStore(fileStorage);
        services.AddSingleton<IDocumentStore>(documentStore);
        services.AddSingleton<IPersisted>(documentStore);
        
        services.AddHostedService<PersistedHostedService>();

        return services;
    }

    private static IServiceCollection AddOtherServices(this IServiceCollection services)
    {
        services.AddSingleton<ITokenizer, Tokenizer>();

        return services;
    }
}