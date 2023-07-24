using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistify.Server.HostedServices;

public class RecurrentServicesHostedService : IHostedService
{
    private readonly CancellationTokenSource _cts = new();
    private readonly ILogger<RecurrentServicesHostedService> _logger;
    private readonly IEnumerable<IActRecurrently> _recurrentActions;
    private readonly List<Task> _runningTasks = new();

    public RecurrentServicesHostedService(
        IEnumerable<IActRecurrently> recurrentActions,
        ILogger<RecurrentServicesHostedService> logger
    )
    {
        _recurrentActions = recurrentActions;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var recurrentAction in _recurrentActions)
        {
            _runningTasks.Add(Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation("Performing recurrent action {RecurrentActionName}", recurrentAction.GetType().Name);
                        await recurrentAction.PerformRecurrentActionAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while performing recurrent action");
                    }

                    try
                    {
                        await Task.Delay(recurrentAction.RecurrentActionInterval, _cts.Token);
                    }
                    catch (TaskCanceledException) { }
                }
            }, cancellationToken));
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel(); // cancel all tasks
        try
        {
            await Task.WhenAll(_runningTasks); // wait for tasks to complete
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while stopping service");
        }
    }
}
