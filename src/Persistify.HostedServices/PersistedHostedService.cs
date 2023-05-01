using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistify.Storage;

namespace Persistify.HostedServices;

public class PersistedHostedService : IHostedService, IDisposable
{
    private readonly ILogger<PersistedHostedService> _logger;
    private readonly IEnumerable<IPersisted> _persisteds;
    private readonly IStorage _storage;
    private Timer? _timer;

    public PersistedHostedService(
        IEnumerable<IPersisted> persisteds,
        IStorage storage,
        ILogger<PersistedHostedService> logger
    )
    {
        _persisteds = persisteds;
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
        foreach (var persisted in _persisteds)
        {
            try
            {
                _logger.LogInformation("Loading {indexerName}", persisted.GetType().Name);
                await persisted.LoadAsync(_storage, cancellationToken);
                _logger.LogInformation("Loaded {indexerName}", persisted.GetType().Name);
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
        foreach (var persisted in _persisteds)
        {
            try
            {
                _logger.LogInformation("Saving {indexerName}", persisted.GetType().Name);
                await persisted.SaveAsync(_storage);
                _logger.LogInformation("Loaded {indexerName}", persisted.GetType().Name);
            }
            catch (Exception e)
            {
                throw new FatalHostedServiceException(e.Message);
            }
        }
    }
}