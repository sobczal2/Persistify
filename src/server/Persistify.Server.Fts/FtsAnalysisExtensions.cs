using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Factories;

namespace Persistify.Server.Fts;

public static class FtsAnalysisExtensions
{
    public static IServiceCollection AddFtsAnalysis(this IServiceCollection services)
    {
        services.AddSingleton<IAnalyzerFactory, AnalyzerFactory>();

        return services;
    }
}
