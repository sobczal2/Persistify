using System.Collections.Generic;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Types;

public class ListTypesPipelineOrchestrator : PipelineOrchestratorBase<ListTypesPipelineOrchestrator, ListTypesPipelineContext, ListTypesRequestProto
    , ListTypesResponseProto>
{
    public ListTypesPipelineOrchestrator(
        IEnumerable<IPipelineMiddleware<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>>
            middlewares,
        ILogger<ListTypesPipelineOrchestrator> logger,
        ISubject<PipelineEventProto> pipelineEventSubject) : base(middlewares, logger, pipelineEventSubject)
    {
    }
}