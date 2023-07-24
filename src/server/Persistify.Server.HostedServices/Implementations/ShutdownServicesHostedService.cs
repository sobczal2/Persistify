using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistify.Server.HostedServices.Abstractions;

namespace Persistify.Server.HostedServices.Implementations;

public class ShutdownServicesHostedService : IHostedService
{
    private readonly ILogger<ShutdownServicesHostedService> _logger;
    private readonly IEnumerable<IActOnShutdown> _shutdownActions;

    public ShutdownServicesHostedService(
        ILogger<ShutdownServicesHostedService> logger,
        IEnumerable<IActOnShutdown> shutdownActions)
    {
        _logger = logger;
        _shutdownActions = shutdownActions;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ShutdownServicesHostedService is starting.");

        foreach (var shutdownAction in _shutdownActions)
        {
            try
            {
                await shutdownAction.PerformShutdownActionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while performing shutdown action");
            }
        }

        _logger.LogInformation("ShutdownServicesHostedService is stopping.");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
