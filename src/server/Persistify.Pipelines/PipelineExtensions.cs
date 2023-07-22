using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Common.Stages;

namespace Persistify.Pipelines;

public static class PipelineExtensions
{
    public static IServiceCollection AddPipelines(this IServiceCollection services)
    {
        services.AddSingleton(typeof(StaticValidationStage<,,>));
        
        return services;
    }
}
