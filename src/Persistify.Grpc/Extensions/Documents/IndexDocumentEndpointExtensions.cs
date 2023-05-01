using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Common;
using Persistify.Pipeline.Middlewares.Documents.Index;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Documents;
using Persistify.Protos;

namespace Persistify.Grpc.Extensions.Documents;

public static class IndexDocumentEndpointExtensions
{
    public static IServiceCollection AddIndexDocumentEndpoint(this IServiceCollection services)
    {
        services.AddOrchestrator();
        services.AddMiddlewares();
        
        return services;
    }
    
    private static void AddOrchestrator(this IServiceCollection services)
    {
        services.AddSingleton<
            IPipelineOrchestrator<IndexDocumentPipelineContext, IndexDocumentRequestProto, IndexDocumentResponseProto>,
            IndexDocumentPipelineOrchestrator
        >();
        
        services.AddSingleton<
            IPipelineOrchestrator,
            IndexDocumentPipelineOrchestrator
        >();
    }
    
    private static void AddMiddlewares(this IServiceCollection services)
    {
        services.TryAddSingleton(typeof(IPipelineMiddleware<,,>), typeof(RequestProtoValidationMiddleware<,,>));
        
        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(FetchTypeFromTypeStoreMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(ParseDataMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(ValidateDataAgainstTypeMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(TokenizeFieldsMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(InsertDocumentIntoDocumentStoreMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(ValidateTokensMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(InsertTokensIntoIndexersMiddleware));
    }
}