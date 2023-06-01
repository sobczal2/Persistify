using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Documents.Search;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Documents;
using Persistify.Protos;

namespace Persistify.Server.Extensions.Documents;

public static class SearchDocumentsEndpointExtensions
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
            IPipelineOrchestrator<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>,
            SearchDocumentsPipelineOrchestrator
        >();

        services.AddSingleton<
            IPipelineOrchestrator,
            SearchDocumentsPipelineOrchestrator
        >();
    }

    private static void AddMiddlewares(this IServiceCollection services)
    {
        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(FetchTypeFromTypeStoreMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(ValidateQueryAgainstTypeMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(SearchIndexesInIndexersMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(ApplyPaginationMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(FetchDocumentsFromDocumentStoreMiddleware));
    }
}