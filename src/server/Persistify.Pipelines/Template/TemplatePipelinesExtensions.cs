using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Template.Contexts;
using Persistify.Pipelines.Template.Pipelines;

namespace Persistify.Pipelines.Template;

internal static class TemplatePipelinesExtensions
{
    public static IServiceCollection AddTemplatePipelines(this IServiceCollection services)
    {
        services.AddPipeline<AddTemplateContext, AddTemplatePipeline>();

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
