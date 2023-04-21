using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Common;
using Persistify.Pipeline.Middlewares.Documents.Search;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Documents;
using Persistify.Protos;

namespace Persistify.Grpc.Extensions.Documents;

public static class SearchDocumentsEndpointExtensions
{
    public static IServiceCollection AddSearchDocumentsEndpoint(this IServiceCollection services)
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
    }

    private static void AddMiddlewares(this IServiceCollection services)
    {
        services.TryAddSingleton(typeof(IPipelineMiddleware<,,>), typeof(RequestProtoValidationMiddleware<,,>));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(ValidateTypeNameMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(TokenizeQueryMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(ValidateTokensMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(SearchIndexesInIndexerMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(FilterIndexesByFieldsMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(RemoveDuplicateIndexesMiddleware));

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