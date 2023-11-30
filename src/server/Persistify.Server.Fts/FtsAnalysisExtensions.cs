using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Factories;
using Persistify.Server.Fts.PresetAnalyzers;

namespace Persistify.Server.Fts;

public static class FtsAnalysisExtensions
{
    public static IServiceCollection AddFtsAnalysis(
        this IServiceCollection services
    )
    {
        services.AddSingleton<IAnalyzerExecutorLookup, AnalyzerExecutorLookup>();
        services.AddTransient<IBuiltInPresetAnalyzer, StandardBuiltInPresetAnalyzer>();

        return services;
    }
}
