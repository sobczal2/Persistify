using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Template.Contexts;
using Persistify.Pipelines.Template.Pipelines;
using Persistify.Pipelines.Template.Stages.AddTemplates;
using Persistify.Pipelines.Template.Stages.ListTemplates;

namespace Persistify.Pipelines.Template;

internal static class TemplatePipelinesExtensions
{
    public static IServiceCollection AddTemplatePipelines(this IServiceCollection services)
    {
        services.AddPipeline<AddTemplateContext, AddTemplatePipeline>();
        services.AddSingleton<ValidateTemplateNameStage>();
        services.AddSingleton<AddTemplateToManagerStage>();

        services.AddPipeline<ListTemplatesContext, ListTemplatesPipeline>();
        services.AddSingleton<FetchTemplatesFromManagerStage>();

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
