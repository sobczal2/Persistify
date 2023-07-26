using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;

namespace Persistify.Server.HostedServices.Implementations;

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

        _logger.LogInformation("Reordering startup actions by dependency");

        var orderedStartupActions = _startupActions
            .OrderByDescending(x => x.GetType().GetStartupPriority())
            .ToList();

        foreach (var startupAction in orderedStartupActions)
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
