using System.Collections.Generic;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Types;

public class CreateTypePipelineOrchestrator : PipelineOrchestratorBase<CreateTypePipelineOrchestrator, CreateTypePipelineContext, CreateTypeRequestProto
    , CreateTypeResponseProto>
{
    public CreateTypePipelineOrchestrator(
        IEnumerable<IPipelineMiddleware<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>>
            middlewares,
        ILogger<CreateTypePipelineOrchestrator> logger,
        ISubject<PipelineEventProto> pipelineEventSubject) : base(middlewares, logger, pipelineEventSubject)
    {
    }
}