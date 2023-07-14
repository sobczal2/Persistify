using Microsoft.Extensions.DependencyInjection;
using Persistify.Management.Bool.Manager;
using Persistify.Management.Fts.Manager;
using Persistify.Management.Number.Manager;
using Persistify.Pipelines.Document.AddDocuments;
using Persistify.Pipelines.Document.AddDocuments.Stages;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;

namespace Persistify.Pipelines.Document;

internal static class DocumentPipelinesExtensions
{
    internal static IServiceCollection AddDocumentPipelines(this IServiceCollection services)
    {
        services.AddPipeline<AddDocumentsContext, AddDocumentsPipeline>();
        services
            .AddSingleton<
                FetchTemplateFromManagerStage<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse>>();
        services.AddSingleton<ValidateDocumentsAgainstTemplateStage>();
        services.AddSingleton<StoreDocumentsStage>();
        services.AddSingleton<AddDocumentsToManagerStage<IFtsManager>>();
        services.AddSingleton<AddDocumentsToManagerStage<IBoolManager>>();
        services.AddSingleton<AddDocumentsToManagerStage<INumberManager>>();

        return services;
    }
}
