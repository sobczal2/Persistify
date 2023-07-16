using Microsoft.Extensions.DependencyInjection;
using Persistify.Analysis.Analyzers;
using Persistify.Analysis.CharacterFilters;
using Persistify.Analysis.TokenFilters;

namespace Persistify.Analysis;

public static class AnalysisExtensions
{
    public static IServiceCollection AddAnalysis(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentAnalyzer, WhitespaceDocumentAnalyzer>();
        services.AddSingleton<IQueryAnalyzer, WhitespaceQueryAnalyzer>();

        services.AddSingleton<ICharacterFilter, SimpleCharacterFilter>();
        services.AddSingleton<ITokenFilter, PassthroughTokenFilter>();

        return services;
    }
}
