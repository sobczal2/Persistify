using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Fts.Analysis.Factories;

namespace Persistify.Server.Fts.Analysis;

public static class FtsAnalysisExtensions
{
    public static IServiceCollection AddFtsAnalysis(this IServiceCollection services)
    {
        services.AddSingleton<IAnalyzerFactory, AnalyzerFactory>();

        return services;
    }
}
