using Microsoft.Extensions.DependencyInjection;
using Persistify.Pipelines.Shared.Stages;

namespace Persistify.Pipelines.Shared;

internal static class SharedPipelinesExtensions
{
    internal static IServiceCollection AddSharedPipelines(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ValidationStage<,,>), typeof(ValidationStage<,,>));
        services.AddSingleton(typeof(FetchTemplateFromManagerStage<,,>), typeof(FetchTemplateFromManagerStage<,,>));

        return services;
    }
}
