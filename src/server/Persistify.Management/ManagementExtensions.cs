using Microsoft.Extensions.DependencyInjection;
using Persistify.Management.Bool.Manager;
using Persistify.Management.Fts.Manager;
using Persistify.Management.Fts.Token;
using Persistify.Management.Number.Manager;
using Persistify.Management.Score;
using Persistify.Management.Template;

namespace Persistify.Management;

public static class ManagementExtensions
{
    public static IServiceCollection AddManagement(this IServiceCollection services)
    {
        services.AddSingleton<IScoreCalculator, LinearScoreCalculator>();
        services.AddSingleton<ITemplateManager, DictionaryTemplateManager>();
        services.AddSingleton<IBoolManager, HashSetBoolManager>();
        services.AddSingleton<INumberManager, IntervalTreeNumberManager>();
        services.AddSingleton<IFtsManager, PrefixTreeFtsManager>();
        services.AddSingleton<ITokenizer, DefaultTokenizer>();

        return services;
    }
}
