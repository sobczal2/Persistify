using Grpc.Core;
using Persistify.Client;
using Persistify.Helpers;
using Persistify.Monitor.Database;
using Persistify.Monitor.Database.Domain;
using Persistify.Monitor.Services;

namespace Persistify.Monitor.HostedServices;

public class PipelineStreamHostedService : BackgroundService
{
    private readonly IPersistifyClient _persistifyClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly StatusProvider _statusProvider;

    public PipelineStreamHostedService(
        IPersistifyClient persistifyClient,
        IServiceScopeFactory serviceScopeFactory,
        StatusProvider statusProvider)
    {
        _persistifyClient = persistifyClient;
        _serviceScopeFactory = serviceScopeFactory;
        _statusProvider = statusProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
            try
            {
                await SubscribeToPipelineStream(cancellationToken);
            }
            catch (RpcException e)
            {
                _statusProvider.PipelineStreamConnected = false;
            }
        }

        _statusProvider.PipelineStreamConnected = false;
    }

    private async Task SubscribeToPipelineStream(CancellationToken cancellationToken)
    {
        await foreach (var pipelineEvent in _persistifyClient.PipelineStreamAsync(cancellationToken))
        {
            _statusProvider.PipelineStreamConnected = true;
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MonitorDbContext>();

            var pipelineEventEntity = new PipelineEvent
            {
                CorrelationId = Guid.Parse(pipelineEvent.CorrelationId),
                PipelineName = pipelineEvent.PipelineName,
                Timestamp = DateTimeHelpers.UnixTimeStampMillisecondsToDateTime(pipelineEvent.Timestamp),
                Duration = TimeSpan.FromMicroseconds(pipelineEvent.DurationUs),
                Success = pipelineEvent.Success,
                FaultedStepName = pipelineEvent.FaultedStepName
            };

            await dbContext.PipelineEvents.AddAsync(pipelineEventEntity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}