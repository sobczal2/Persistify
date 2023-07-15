using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Template.AddTemplate;
using Persistify.Pipelines.Template.AddTemplate.Stages;
using Persistify.Pipelines.Template.DeleteTemplate;
using Persistify.Pipelines.Template.DeleteTemplate.Stages;
using Persistify.Pipelines.Template.ListTemplate;
using Persistify.Pipelines.Template.ListTemplate.Stages;
using CheckTemplateNameStage = Persistify.Pipelines.Template.AddTemplate.Stages.CheckTemplateNameStage;

namespace Persistify.Pipelines.Template;

internal static class TemplatePipelinesExtensions
{
    internal static IServiceCollection AddTemplatePipelines(this IServiceCollection services)
    {
        services.AddSingleton<AddTemplatePipeline>();
        services.AddSingleton<CheckTemplateNameStage>();
        services.AddSingleton<AddTemplateToManagerStage>();

        services.AddSingleton<ListTemplatesPipeline>();
        services.AddSingleton<FetchTemplatesFromManagerStage>();

        services.AddSingleton<DeleteTemplatePipeline>();
        services.AddSingleton<DeleteTemplate.Stages.CheckTemplateNameStage>();
        services.AddSingleton<DeleteTemplateFromTemplateManagerStage>();
        services.AddSingleton<DeleteTemplateFromDocumentManagerStage>();

        return services;
    }
}
