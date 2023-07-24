using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Fts.Analysis.Factories;
using Persistify.Server.Fts.Analysis.Presets;

namespace Persistify.Server.Fts.Analysis;

public static class FtsAnalysisExtensions
{
    public static IServiceCollection AddFtsAnalysis(this IServiceCollection services)
    {
        services.AddSingleton<IAnalyzerFactory, StandardAnalyzerFactory>();
        services.AddSingleton<IAnalyzerPresetFactory, StandardAnalyzerPresetFactory>();

        return services;
    }
}
