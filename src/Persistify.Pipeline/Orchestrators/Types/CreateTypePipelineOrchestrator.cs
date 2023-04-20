using System.Collections.Generic;
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
        ILogger<CreateTypePipelineOrchestrator> logger) : base(middlewares, logger)
    {
    }
}