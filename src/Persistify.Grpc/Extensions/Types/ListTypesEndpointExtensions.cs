using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Common;
using Persistify.Pipeline.Middlewares.Types.List;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Types;
using Persistify.Protos;

namespace Persistify.Grpc.Extensions.Types;

public static class ListTypesEndpointExtensions
{
    public static IServiceCollection AddListTypesEndpoint(this IServiceCollection services)
    {
        services.AddOrchestrator();
        services.AddMiddlewares();
        
        return services;
    }
    
    private static void AddOrchestrator(this IServiceCollection services)
    {
        services.AddSingleton<
            IPipelineOrchestrator<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>,
            ListTypesPipelineOrchestrator
        >();
        
        services.AddSingleton<
            IPipelineOrchestrator,
            ListTypesPipelineOrchestrator
        >();
    }
    
    private static void AddMiddlewares(this IServiceCollection services)
    {
        services.TryAddSingleton(typeof(IPipelineMiddleware<,,>), typeof(RequestProtoValidationMiddleware<,,>));
        
        services.AddSingleton(
            typeof(IPipelineMiddleware<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>),
            typeof(GetTypesFromTypeStoreMiddleware));
    }
}