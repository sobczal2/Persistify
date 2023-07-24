using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Pipelines.Documents.GetDocument;
using Persistify.Server.Pipelines.Documents.GetDocument.Stages;
using Persistify.Server.Pipelines.Documents.IndexDocument;
using Persistify.Server.Pipelines.Documents.IndexDocument.Stages;

namespace Persistify.Server.Pipelines.Documents;

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
