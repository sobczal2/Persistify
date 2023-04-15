using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistify.Storage;
using Persistify.Stores.Common;

namespace Persistify.HostedServices;

public class PersistedStoreHostedService : IHostedService, IDisposable
{
    private readonly ILogger<PersistedStoreHostedService> _logger;
    private readonly IEnumerable<IPersistedStore> _persistedStores;
    private readonly IStorage _storage;
    private Timer? _timer;

    public PersistedStoreHostedService(
        IEnumerable<IPersistedStore> persistedStores,
        IStorage storage,
        ILogger<PersistedStoreHostedService> logger
    )
    {
        _persistedStores = persistedStores;
        _storage = storage;
        _logger = logger;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting persisted store hosted service");
        foreach (var store in _persistedStores)
            try
            {
                await store.LoadAsync(_storage, cancellationToken);
            }
            catch (Exception e)
            {
                throw new FatalHostedServiceException(e.Message);
            }

        _timer = new Timer(TimerExecuteSaveAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping persisted store hosted service");
        _timer?.Change(Timeout.Infinite, 0);

        await ExecuteSaveAsync();
    }

    private async void TimerExecuteSaveAsync(object? state)
    {
        await ExecuteSaveAsync();
    }

    private async Task ExecuteSaveAsync()
    {
        foreach (var store in _persistedStores)
            try
            {
                await store.SaveAsync(_storage);
            }
            catch (Exception e)
            {
                throw new FatalHostedServiceException(e.Message);
            }
    }
}