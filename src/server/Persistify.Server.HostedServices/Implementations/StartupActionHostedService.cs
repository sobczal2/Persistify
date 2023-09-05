using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistify.Server.HostedServices.Abstractions;

namespace Persistify.Server.HostedServices.Implementations;

public class StartupActionHostedService : BackgroundService
{
    private readonly IEnumerable<IStartupAction> _startupActions;
    private readonly ILogger<StartupActionHostedService> _logger;

    public StartupActionHostedService(
        IEnumerable<IStartupAction> startupActions,
        ILogger<StartupActionHostedService> logger
        )
    {
        _startupActions = startupActions;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();

        foreach (var startupAction in _startupActions)
        {
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("Performing startup action {StartupActionName}", startupAction.Name);
                    await startupAction.PerformStartupActionAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while performing startup action {StartupActionName}", startupAction.Name);
                }
            }, stoppingToken));
        }

        await Task.WhenAll(tasks);
    }
}
