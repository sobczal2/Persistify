using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Management.Types.Bool;
using Persistify.Server.Management.Types.Fts;
using Persistify.Server.Management.Types.Number;
using Persistify.Server.Management.Types.Number.Queries;
using Persistify.Server.Management.Types.Text;

namespace Persistify.Server.Management.Types;

public static class ManagementTypesExtensions
{
    public static IServiceCollection AddManagementTypes(this IServiceCollection services)
    {
        services.AddSingleton<BoolManager>();
        services.AddSingleton<FtsManager>();
        services.AddSingleton<NumberManager>();
        services.AddSingleton<TextManager>();

        services.AddSingleton<ITypeManager>(sp => sp.GetRequiredService<BoolManager>());
        services.AddSingleton<ITypeManager>(sp => sp.GetRequiredService<FtsManager>());
        services.AddSingleton<ITypeManager>(sp => sp.GetRequiredService<NumberManager>());
        services.AddSingleton<ITypeManager>(sp => sp.GetRequiredService<TextManager>());

        services.AddSingleton<ITypeManager<BoolManagerQuery, BoolManagerHit>>(sp => sp.GetRequiredService<BoolManager>());
        services.AddSingleton<ITypeManager<FtsManagerQuery, FtsManagerHit>>(sp => sp.GetRequiredService<FtsManager>());
        services.AddSingleton<ITypeManager<NumberManagerQuery, NumberManagerHit>>(sp => sp.GetRequiredService<NumberManager>());
        services.AddSingleton<ITypeManager<TextManagerQuery, TextManagerHit>>(sp => sp.GetRequiredService<TextManager>());

        services.AddSingleton<IActOnStartup>(sp => sp.GetRequiredService<BoolManager>());
        services.AddSingleton<IActOnStartup>(sp => sp.GetRequiredService<FtsManager>());
        services.AddSingleton<IActOnStartup>(sp => sp.GetRequiredService<NumberManager>());
        services.AddSingleton<IActOnStartup>(sp => sp.GetRequiredService<TextManager>());


        return services;
    }
}
