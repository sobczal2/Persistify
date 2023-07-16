using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Document.AddDocuments;
using Persistify.Pipelines.Document.AddDocuments.Stages;
using Persistify.Pipelines.Document.GetDocument;
using Persistify.Pipelines.Document.GetDocument.Stages;

namespace Persistify.Pipelines.Document;

internal static class DocumentPipelinesExtensions
{
    internal static IServiceCollection AddDocumentPipelines(this IServiceCollection services)
    {
        services.AddSingleton<AddDocumentsPipeline>();
        services.AddSingleton<ValidateDocumentsAgainstTemplateStage>();
        services.AddSingleton<StoreDocumentsStage>();

        services.AddSingleton<GetDocumentPipeline>();
        services.AddSingleton<VerifyTemplateExistsStage>();
        services.AddSingleton<FetchDocumentFromDocumentManagerStage>();

        return services;
    }
}
