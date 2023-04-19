using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistify.Indexes.Common;
using Persistify.Storage;
using Persistify.Stores.Common;

namespace Persistify.HostedServices;

public class PersistedHostedService : IHostedService, IDisposable
{
    private readonly ILogger<PersistedStoreHostedService> _logger;
    private readonly IEnumerable<IPersistedIndexer> _persistedIndexers;
    private readonly IStorage _storage;
    private Timer? _timer;

    public PersistedHostedService(
        IEnumerable<IPersistedIndexer> persistedIndexers,
        IStorage storage,
        ILogger<PersistedStoreHostedService> logger
    )
    {
        _persistedIndexers = persistedIndexers;
        _storage = storage;
        _logger = logger;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {name}", GetType().Name);
        foreach (var indexer in _persistedIndexers)
        {
            try
            {
                _logger.LogInformation("Attempting to load {indexerName}", indexer.GetType().Name);
                await indexer.LoadAsync(_storage, cancellationToken);
            }
            catch (Exception e)
            {
                throw new FatalHostedServiceException(e.Message);
            }
        }

        _timer = new Timer(TimerExecuteSaveAsync, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping {name}", GetType().Name);
        _timer?.Change(Timeout.Infinite, 0);

        await ExecuteSaveAsync();
    }

    private async void TimerExecuteSaveAsync(object? state)
    {
        await ExecuteSaveAsync();
    }

    private async Task ExecuteSaveAsync()
    {
        foreach (var indexer in _persistedIndexers)
        {
            _logger.LogInformation("Attempting to save {storeName}", indexer.GetType().Name);
            try
            {
                await indexer.SaveAsync(_storage);
            }
            catch (Exception e)
            {
                throw new FatalHostedServiceException(e.Message);
            }
        }
    }
}