using Microsoft.Extensions.DependencyInjection;
using Persistify.Fts.Analysis.Abstractions;
using Persistify.Fts.Analysis.Factories;
using Persistify.Fts.Analysis.Presets;

namespace Persistify.Fts.Analysis;

public static class FtsAnalysisExtensions
{
    public static IServiceCollection AddFtsAnalysis(this IServiceCollection services)
    {
        services.AddSingleton<IAnalyzerFactory, StandardAnalyzerFactory>();
        services.AddSingleton<IAnalyzerPresetFactory, StandardAnalyzerPresetFactory>();

        return services;
    }
}
