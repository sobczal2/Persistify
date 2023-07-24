using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Pipelines.Common.Stages;

namespace Persistify.Server.Pipelines;

public static class PipelineExtensions
{
    public static IServiceCollection AddPipelines(this IServiceCollection services)
    {
        services.AddSingleton(typeof(StaticValidationStage<,,>));
        
        return services;
    }
}
