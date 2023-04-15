using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Grpc.Interceptors;
using Persistify.Grpc.Services;
using Persistify.HostedServices;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Common;
using Persistify.Pipeline.Middlewares.Types;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Types;
using Persistify.Pipeline.Wrappers.Abstractions;
using Persistify.Pipeline.Wrappers.Common;
using Persistify.Protos;
using Persistify.RequestValidators.Types;
using Persistify.Storage;
using Persistify.Stores.Common;
using Persistify.Stores.Documents;
using Persistify.Stores.Types;
using Serilog;

namespace Persistify.Grpc;

public static class PersistifyExtensions
{
    public static IServiceCollection AddPersistify(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(opt => { opt.Interceptors.Add<ValidationInterceptor>(); });
        services.AddGrpcReflection();

        services.AddPipeline();
        services.AddValidatorsFromAssemblyContaining<CreateTypeRequestProtoValidator>(ServiceLifetime.Singleton);
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

    private static IServiceCollection AddPipeline(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ICommonMiddlewareWrapper<,,>), typeof(TimeLoggingMiddlewareWrapper<,,>));
        services.AddSingleton(typeof(RequestProtoValidationMiddleware<,,>));

        services.AddSingleton<
            IPipelineOrchestrator<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>,
            CreateTypePipelineOrchestrator
        >();

        services.AddSingleton<
            IPipelineOrchestrator<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>,
            ListTypesPipelineOrchestrator
        >();

        services.AddSingleton<AddTypeToStoreMiddleware>();
        services.AddSingleton<ListTypesFromStoreMiddleware>();


        return services;
    }

    private static IServiceCollection AddStores(this IServiceCollection services)
    {
        var typeStore = new HashSetTypeStore();
        services.AddSingleton<ITypeStore>(typeStore);
        services.AddSingleton<IPersistedStore>(typeStore);


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
        return services;
    }
}