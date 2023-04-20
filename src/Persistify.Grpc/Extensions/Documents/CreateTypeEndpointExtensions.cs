using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Common;
using Persistify.Pipeline.Middlewares.Types.Create;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Types;
using Persistify.Protos;

namespace Persistify.Grpc.Extensions.Documents;

public static class CreateTypeEndpointExtensions
{
    public static IServiceCollection AddCreateTypeEndpoint(this IServiceCollection services)
    {
        services.AddOrchestrator();
        services.AddMiddlewares();
        
        return services;
    }

    private static void AddOrchestrator(this IServiceCollection services)
    {
        services.AddSingleton<
            IPipelineOrchestrator<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>,
            CreateTypePipelineOrchestrator
        >();
    }

    private static void AddMiddlewares(this IServiceCollection services)
    {
        services.TryAddSingleton(typeof(IPipelineMiddleware<,,>), typeof(RequestProtoValidationMiddleware<,,>));
        
        services.AddSingleton(
            typeof(IPipelineMiddleware<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>),
            typeof(AddTypeToTypeStoreMiddleware));
    }
}