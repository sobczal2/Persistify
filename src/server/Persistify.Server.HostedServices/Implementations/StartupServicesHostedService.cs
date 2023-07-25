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
        var orderedStartupActions = ReorderByDependency();

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

    private IEnumerable<IActOnStartup> ReorderByDependency()
    {
        var orderedStartupActions = new List<IActOnStartup>();
        var startupActionsToOrder = _startupActions.ToList();

        for(var i = 0; i < startupActionsToOrder.Count; i++)
        {
            if(orderedStartupActions.Contains(startupActionsToOrder[i]))
            {
                continue;
            }

            orderedStartupActions = AddDependency(startupActionsToOrder[i], orderedStartupActions);
        }

        return orderedStartupActions;
    }

    private List<IActOnStartup> AddDependency(IActOnStartup actionToAdd, List<IActOnStartup> orderedStartupActions)
    {
        if(!actionToAdd.GetType().GetStartupDependencies().Any())
        {
            orderedStartupActions.Add(actionToAdd);
            return orderedStartupActions;
        }

        foreach(var dependency in actionToAdd.GetType().GetStartupDependencies())
        {
            var dependencyAction = _startupActions.FirstOrDefault(x => x.GetType() == dependency);
            if(dependencyAction is null)
            {
                throw new Exception($"Startup action {actionToAdd.GetType().Name} has a dependency on {dependency.Name} but {dependency.Name} is not registered as a startup action");
            }

            orderedStartupActions = AddDependency(dependencyAction, orderedStartupActions);
        }

        orderedStartupActions.Add(actionToAdd);
        return orderedStartupActions;
    }
}
