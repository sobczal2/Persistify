using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Document;
using Persistify.Pipelines.Shared;
using Persistify.Pipelines.Template;

namespace Persistify.Pipelines;

public static class PipelineExtensions
{
    public static IServiceCollection AddPipelines(this IServiceCollection services)
    {
        services.AddSharedPipelines();
        services.AddTemplatePipelines();
        services.AddDocumentPipelines();

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
