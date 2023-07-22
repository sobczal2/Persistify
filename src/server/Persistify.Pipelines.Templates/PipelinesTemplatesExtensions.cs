using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Templates.CreateTemplates;
using Persistify.Pipelines.Templates.CreateTemplates.Stages;
using Persistify.Pipelines.Templates.GetTemplates;
using Persistify.Pipelines.Templates.GetTemplates.Stages;
using Persistify.Pipelines.Templates.ListTemplates;
using Persistify.Pipelines.Templates.ListTemplates.Stages;

namespace Persistify.Pipelines.Templates;

public static class PipelinesTemplatesExtensions
{
    public static IServiceCollection AddPipelinesTemplates(this IServiceCollection services)
    {
        services.AddSingleton(typeof(CreateTemplatePipeline));
        services.AddSingleton(typeof(GetTemplatePipeline));
        services.AddSingleton(typeof(ListTemplatesPipeline));

        services.AddSingleton(typeof(CheckAnalyzersAvailabilityStage));
        services.AddSingleton(typeof(AddTemplateToTemplateManagerStage));

        services.AddSingleton(typeof(FetchTemplateFromTemplateManagerStage));

        services.AddSingleton(typeof(FetchTemplatesFromTemplateManagerStage));
        services.AddSingleton(typeof(ApplyPaginationStage));

        return services;
    }
}
