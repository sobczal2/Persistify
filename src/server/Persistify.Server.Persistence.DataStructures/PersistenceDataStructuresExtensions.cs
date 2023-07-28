using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Persistence.DataStructures.Abstractions;
using Persistify.Server.Persistence.DataStructures.Providers;

namespace Persistify.Server.Persistence.DataStructures;

public static class PersistenceDataStructuresExtensions
{
    public static IServiceCollection AddPersistenceDataStructures(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IStorageProvider<>), typeof(RepositoryStorageProvider<>));
        services.AddSingleton<IStorageProviderManager, RepositoryStorageProviderManager>();

        return services;
    }
}
