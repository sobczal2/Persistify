using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Shared;
using Persistify.Pipelines.Template;

namespace Persistify.Pipelines;

public static class PipelineExtensions
{
    public static IServiceCollection AddPipelines(this IServiceCollection services)
    {
        services.AddSharedPipelines();
        services.AddTemplatePipelines();

        return services;
    }
}
