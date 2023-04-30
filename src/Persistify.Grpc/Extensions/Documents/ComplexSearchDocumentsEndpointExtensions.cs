using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Documents.ComplexSearch;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Documents;
using Persistify.Protos;

namespace Persistify.Grpc.Extensions.Documents;

public static class ComplexSearchDocumentsEndpointExtensions
{
    public static IServiceCollection AddComplexSearchDocumentsEndpoint(this IServiceCollection services)
    {
        services.AddOrchestrator();
        services.AddMiddlewares();

        return services;
    }

    private static void AddOrchestrator(this IServiceCollection services)
    {
        services.AddSingleton<
            IPipelineOrchestrator<ComplexSearchDocumentsPipelineContext, ComplexSearchDocumentsRequestProto,
                ComplexSearchDocumentsResponseProto>,
            ComplexSearchDocumentsPipelineOrchestrator
        >();
    }

    private static void AddMiddlewares(this IServiceCollection services)
    {
        services.AddSingleton(
            typeof(IPipelineMiddleware<ComplexSearchDocumentsPipelineContext, ComplexSearchDocumentsRequestProto,
                ComplexSearchDocumentsResponseProto>),
            typeof(FetchTypeFromTypeStoreMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<ComplexSearchDocumentsPipelineContext, ComplexSearchDocumentsRequestProto,
                ComplexSearchDocumentsResponseProto>),
            typeof(ValidateQueryAgainstTypeMiddleware));
        
        services.AddSingleton(
            typeof(IPipelineMiddleware<ComplexSearchDocumentsPipelineContext, ComplexSearchDocumentsRequestProto,
                ComplexSearchDocumentsResponseProto>),
            typeof(SearchIndexesInIndexersMiddleware));
    }
}