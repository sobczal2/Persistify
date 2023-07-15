using Microsoft.Extensions.DependencyInjection;
using Persistify.Cache;
using Persistify.Management.Bool.Manager;
using Persistify.Management.Document.Cache;
using Persistify.Management.Document.Manager;
using Persistify.Management.Fts.Manager;
using Persistify.Management.Fts.Token;
using Persistify.Management.Number.Manager;
using Persistify.Management.Score;
using Persistify.Management.Template.Cache;
using Persistify.Management.Template.Manager;

namespace Persistify.Management;

public static class ManagementExtensions
{
    public static IServiceCollection AddManagement(this IServiceCollection services)
    {
        services.AddSingleton<ITemplateManager, DictionaryTemplateManager>();
        services.AddSingleton<IDocumentManager, DocumentManager>();
        services.AddSingleton<IBoolManager, HashSetBoolManager>();
        services.AddSingleton<INumberManager, IntervalTreeNumberManager>();
        services.AddSingleton<IFtsManager, PrefixTreeFtsManager>();
        services.AddSingleton<ITokenizer, DefaultTokenizer>();
        services.AddSingleton<IScoreCalculator, LinearScoreCalculator>();

        services.AddCache<ITemplateCache, TemplateCache>();
        services.AddCache<IDocumentCache, DocumentCache>();

        return services;
    }
}
