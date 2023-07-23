using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Documents.GetDocument;
using Persistify.Pipelines.Documents.GetDocument.Stages;
using Persistify.Pipelines.Documents.IndexDocument;
using Persistify.Pipelines.Documents.IndexDocument.Stages;

namespace Persistify.Pipelines.Documents;

public static class PipelineDocumentsExtensions
{
    public static IServiceCollection AddPipelinesDocuments(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IndexDocumentPipeline));
        services.AddSingleton(typeof(GetDocumentPipeline));

        services.AddSingleton(typeof(IndexDocumentInDocumentManagerStage));

        services.AddSingleton(typeof(GetDocumentFromDocumentManagerStage));

        return services;
    }
}
