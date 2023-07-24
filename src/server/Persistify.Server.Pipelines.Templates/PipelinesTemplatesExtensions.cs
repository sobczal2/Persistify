using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Pipelines.Templates.CreateTemplate;
using Persistify.Server.Pipelines.Templates.CreateTemplate.Stages;
using Persistify.Server.Pipelines.Templates.DeleteTemplate;
using Persistify.Server.Pipelines.Templates.DeleteTemplate.Stages;
using Persistify.Server.Pipelines.Templates.GetTemplate;
using Persistify.Server.Pipelines.Templates.GetTemplate.Stages;
using Persistify.Server.Pipelines.Templates.ListTemplates;
using Persistify.Server.Pipelines.Templates.ListTemplates.Stages;

namespace Persistify.Server.Pipelines.Templates;

public static class PipelinesTemplatesExtensions
{
    public static IServiceCollection AddPipelinesTemplates(this IServiceCollection services)
    {
        services.AddSingleton(typeof(CreateTemplatePipeline));
        services.AddSingleton(typeof(GetTemplatePipeline));
        services.AddSingleton(typeof(ListTemplatesPipeline));
        services.AddSingleton(typeof(DeleteTemplatePipeline));

        services.AddSingleton(typeof(CheckAnalyzersAvailabilityStage));
        services.AddSingleton(typeof(AddTemplateToTemplateManagerStage));

        services.AddSingleton(typeof(FetchTemplateFromTemplateManagerStage));

        services.AddSingleton(typeof(FetchTemplatesFromTemplateManagerStage));
        services.AddSingleton(typeof(ApplyPaginationStage));

        services.AddSingleton(typeof(DeleteTemplateFromTemplateManagerStage));

        return services;
    }
}
