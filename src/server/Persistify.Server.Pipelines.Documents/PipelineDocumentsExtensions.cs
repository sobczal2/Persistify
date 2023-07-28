using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Pipelines.Documents.GetDocument;
using Persistify.Server.Pipelines.Documents.GetDocument.Stages;
using Persistify.Server.Pipelines.Documents.IndexDocument;
using Persistify.Server.Pipelines.Documents.IndexDocument.Stages;
using Persistify.Server.Pipelines.Documents.SearchDocuments;
using Persistify.Server.Pipelines.Documents.SearchDocuments.Stages;

namespace Persistify.Server.Pipelines.Documents;

public static class PipelineDocumentsExtensions
{
    public static IServiceCollection AddPipelinesDocuments(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IndexDocumentPipeline));
        services.AddSingleton(typeof(GetDocumentPipeline));
        services.AddSingleton(typeof(SearchDocumentsPipeline));

        services.AddSingleton(typeof(IndexDocumentInDocumentManagerStage));

        services.AddSingleton(typeof(GetDocumentFromDocumentManagerStage));

        services.AddSingleton(typeof(ValidateQueryAgainstTemplateStage));
        services.AddSingleton(typeof(EvaluateQueryStage));
        services.AddSingleton(typeof(SortDocumentScoresStage));
        services.AddSingleton(typeof(ApplyPaginationStage));
        services.AddSingleton(typeof(FetchDocumentsFromDocumentStoreStage));

        return services;
    }
}
