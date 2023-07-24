using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistify.Server.HostedServices;

public class StartupServicesHostedService : IHostedService
{
    private readonly ILogger<StartupServicesHostedService> _logger;
    private readonly IEnumerable<IActOnStartup> _startupActions;

    public StartupServicesHostedService(
        IEnumerable<IActOnStartup> startupActions,
        ILogger<StartupServicesHostedService> logger
    )
    {
        _startupActions = startupActions;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartupServicesHostedService is starting");
        foreach (var startupAction in _startupActions)
        {
            try
            {
                _logger.LogInformation("Performing startup action {StartupActionName}", startupAction.GetType().Name);
                await startupAction.PerformStartupActionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while performing startup action");
            }
        }

        _logger.LogInformation("StartupServicesHostedService is stopping");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
