using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Server.Services;

// [Authorize(Roles = UserRoles.SuperUser)]
public class MonitorService : Protos.MonitorService.MonitorServiceBase
{
    private readonly ISubject<PipelineEventProto> _pipelineEventSubject;
    private readonly IEnumerable<IPipelineOrchestrator> _pipelineOrchestrators;

    public MonitorService(
        ISubject<PipelineEventProto> pipelineEventSubject,
        IEnumerable<IPipelineOrchestrator> pipelineOrchestrators)
    {
        _pipelineEventSubject = pipelineEventSubject;
        _pipelineOrchestrators = pipelineOrchestrators;
    }

    public override async Task PipelineStream(EmptyProto request,
        IServerStreamWriter<PipelineEventProto> responseStream,
        ServerCallContext context)
    {
        var subscription = _pipelineEventSubject.Subscribe(@event => { responseStream.WriteAsync(@event); });

        try
        {
            await Task.Delay(-1, context.CancellationToken);
        }
        catch (OperationCanceledException)
        {
            // ignored
        }

        subscription.Dispose();
    }

    public override Task<PipelineInfosResponseProto> PipelineInfos(EmptyProto request, ServerCallContext context)
    {
        var pipelineStatusResponse = new PipelineInfosResponseProto();
        foreach (var pipelineOrchestrator in _pipelineOrchestrators)
        {
            var (name, middlewareNames) = pipelineOrchestrator.GetPipelineInfo();
            pipelineStatusResponse.Pipelines.Add(new PipelineInfoProto
            {
                Name = name,
                MiddlewareNames = { middlewareNames }
            });
        }

        return Task.FromResult(pipelineStatusResponse);
    }
}