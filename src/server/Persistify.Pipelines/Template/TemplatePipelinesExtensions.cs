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
        services.AddPipeline<AddTemplateContext, AddTemplatePipeline>();
        services.AddSingleton<CheckTemplateNameStage>();
        services.AddSingleton<AddTemplateToManagerStage>();

        services.AddPipeline<ListTemplatesContext, ListTemplatesPipeline>();
        services.AddSingleton<FetchTemplatesFromManagerStage>();

        services.AddPipeline<DeleteTemplateContext, DeleteTemplatePipeline>();
        services.AddSingleton<DeleteTemplate.Stages.CheckTemplateNameStage>();
        services.AddSingleton<DeleteTemplateFromManagerStage>();

        return services;
    }

    internal static IServiceCollection AddPipeline<TContext, TPipeline>(this IServiceCollection services)
        where TContext : class
        where TPipeline : class
    {
        services.AddSingleton<TPipeline>();
        services.AddScoped<TContext>();

        return services;
    }
}
