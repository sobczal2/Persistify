using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Pipelines;

public static class PipelineExtensions
{
    public static IServiceCollection AddPipelines(this IServiceCollection services)
    {
        return services;
    }
}
