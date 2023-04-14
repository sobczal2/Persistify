using System.Linq;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie.Mappers;
using Persistify.Dtos.Validators;
using Persistify.Grpc.Services;
using Persistify.HostedServices;
using Persistify.PipelineBehaviours;
using Persistify.ProtoMappers;
using Persistify.Storage;
using Persistify.Stores.Common;
using Persistify.Stores.Documents;
using Persistify.Stores.Objects;
using Persistify.Stores.Types;
using Persistify.Tokenizer;
using Serilog;

namespace Persistify.Grpc;

public static class PersistifyExtensions
{
    public static IServiceCollection AddPersistify(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        services.AddGrpcReflection();

        services.AddMediatorServices();
        services.AddProtoMappers();
        services.AddDtoValidators();
        services.AddStores();
        services.AddStorage();
        services.AddOtherServices();

        return services;
    }

    public static WebApplication UsePersistify(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.MapGrpcReflectionService();

        app.MapGrpcService<TypeService>();
        app.MapGrpcService<ObjectService>();

        app.MapGet("/", () => "Use gRPC client to call the service");

        return app;
    }

    private static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        services.AddMediator();
#if DEBUG
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(TimeLoggingBehaviour<,>));
#endif

        return services;
    }

    private static IServiceCollection AddProtoMappers(this IServiceCollection services)
    {
        var mapperInterfaceType = typeof(IProtoMapper<,>);
        var mapperImplTypes = typeof(IProtoMapper<,>).Assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == mapperInterfaceType));

        foreach (var mapperImplType in mapperImplTypes)
            services.AddSingleton(
                mapperImplType.GetInterfaces()
                    .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == mapperInterfaceType),
                mapperImplType);

        return services;
    }

    private static IServiceCollection AddDtoValidators(this IServiceCollection services)
    {
        var validatorInterfaceType = typeof(IValidator<>);
        var validatorImplTypes = typeof(IValidator<>).Assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterfaceType));

        foreach (var validatorImplType in validatorImplTypes)
            services.AddSingleton(
                validatorImplType.GetInterfaces().Single(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterfaceType), validatorImplType);

        return services;
    }

    private static IServiceCollection AddStores(this IServiceCollection services)
    {
        var typeStore = new HashSetTypeStore();
        services.AddSingleton<ITypeStore>(typeStore);
        services.AddSingleton<IPersistedStore>(typeStore);

        var indexStore = new TrieIndexStore(new StandardCaseSensitiveSingleTargetMapper());
        services.AddSingleton<IIndexStore>(indexStore);

        services.AddSingleton<IDocumentStore, StorageDocumentStore>();

        services.AddHostedService<PersistedStoreHostedService>();

        return services;
    }

    private static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddTransient<IStorage>(_ => new CompressingFileSystemStorage("/home/sobczal/temp"));

        return services;
    }

    private static IServiceCollection AddOtherServices(this IServiceCollection services)
    {
        services.AddSingleton<ITokenizer, CaseSensitiveTokenizer>();

        return services;
    }
}