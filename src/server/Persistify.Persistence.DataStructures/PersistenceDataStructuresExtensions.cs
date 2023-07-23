using Microsoft.Extensions.DependencyInjection;
using Persistify.Persistence.DataStructures.Abstractions;
using Persistify.Persistence.DataStructures.Factories;
using Persistify.Persistence.DataStructures.Providers;

namespace Persistify.Persistence.DataStructures;

public static class PersistenceDataStructuresExtensions
{
    public static IServiceCollection AddPersistenceDataStructures(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IStorageProvider<>), typeof(RepositoryStorageProvider<>));
        services.AddSingleton<IStorageProviderFactory, RepositoryStorageProviderFactory>();
        services.AddSingleton<IAsyncTreeFactory, AvlAsyncTreeFactory>();

        return services;
    }
}
