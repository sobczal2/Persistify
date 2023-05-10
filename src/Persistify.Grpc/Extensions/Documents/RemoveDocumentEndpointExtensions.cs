using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Common;
using Persistify.Pipeline.Middlewares.Documents.Remove;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Documents;
using Persistify.Protos;

namespace Persistify.Grpc.Extensions.Documents;

public static class RemoveDocumentEndpointExtensions
{
    public static IServiceCollection AddRemoveDocumentEndpoint(this IServiceCollection services)
    {
        services.AddOrchestrator();
        services.AddMiddlewares();

        return services;
    }

    private static void AddOrchestrator(this IServiceCollection services)
    {
        services.AddSingleton<
            IPipelineOrchestrator<RemoveDocumentPipelineContext, RemoveDocumentRequestProto,
                RemoveDocumentResponseProto>,
            RemoveDocumentPipelineOrchestrator
        >();

        services.AddSingleton<
            IPipelineOrchestrator,
            RemoveDocumentPipelineOrchestrator
        >();
    }

    private static void AddMiddlewares(this IServiceCollection services)
    {
        services.TryAddSingleton(typeof(IPipelineMiddleware<,,>), typeof(RequestProtoValidationMiddleware<,,>));

        services.AddSingleton(
            typeof(IPipelineMiddleware<RemoveDocumentPipelineContext, RemoveDocumentRequestProto,
                RemoveDocumentResponseProto>),
            typeof(ValidateTypeNameMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<RemoveDocumentPipelineContext, RemoveDocumentRequestProto,
                RemoveDocumentResponseProto>),
            typeof(RemoveDocumentFromIndexersMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<RemoveDocumentPipelineContext, RemoveDocumentRequestProto,
                RemoveDocumentResponseProto>),
            typeof(RemoveDocumentFromDocumentStoreMiddleware));
    }
}